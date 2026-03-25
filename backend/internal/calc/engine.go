/*
 * CALMAR EQUILIBRIUM ENGINE - CORE COMPILATION
 * --------------------------------------------
 * Ce fichier implémente l'algorithme itératif d'équilibre statique
 * pour determiner l'enfoncement (Draft) et la tension de mouillage.
 *
 * Algorithme :
 * 1. Incrémentation de l'enfoncement (submersion step 5mm).
 * 2. Calcul du volume immergé cumulé Structure + Flotteur.
 * 3. Évaluation des efforts de traînées environnementales (IALA).
 * 4. Convergence vers l'équilibre : Poussée d'Archimède >= Poids Totaux.
 */
package calc

import (
	"backend/internal/models"
	"fmt"
	"math"
	"time"
)

// SiteConditions représente les variables météo saisies par l'utilisateur
type SiteConditions struct {
	WaterDepth      float64 // Profondeur (H) en mètres
	WindVelocity    float64 // Vitesse du vent (V_wind) en m/s
	CurrentVelocity float64 // Vitesse du courant (V_current) en m/s
	WaveHeight      float64 // Hauteur de la houle (H_wave) en mètres
	WavePeriod      float64 // Période de la houle (T) en secondes
	Marnage         float64 // Amplitude de marée (marnage) en mètres
}

// SimulationParams regroupe les choix de configuration de l'utilisateur
type SimulationParams struct {
	Buoy          models.Buoy
	Chain         models.Chain
	NumBallast    int     // Nombre de lests
	LestDensity   float64 // kg/m³
	AnchorDensity float64 // kg/m³
}

// FindEquilibrium fait tourner la boucle d'équilibre pas-à-pas
func FindEquilibrium(params SimulationParams, conditions SiteConditions) (SimulationResult, error) {
	fmt.Printf("🔬 Simulation IALA : %s (Chaîne: %s DN%.1f)\n", 
		params.Buoy.Name, params.Chain.Type, params.Chain.DN)

	startTime := time.Now()
	var result SimulationResult

	// 1. Paramètres caténaires
	submersionStep := 0.005 
	currentDepth := 0.0     
	maxDepthAllowed := 25.0 

	poidsLineicImmerge := CalculateLineicWeight(params.Chain.MasseLineique)
	hauteurCatenaire := conditions.WaterDepth 

	diametreChaineM := params.Chain.DN / 1000.0
	surfaceChaineRef := hauteurCatenaire * (diametreChaineM * 2.65)

	// 2. Masses Fixes
	masseBoueeFixe := calculateTotalMass(params.Buoy)
	masseLestTotale := float64(params.NumBallast) * params.Buoy.MasseLestUnitaire
	poidsLestImmerge := CalculateSubmergedBallastWeight(masseLestTotale)

	// 3. Boucle d'Équilibre
	for currentDepth < maxDepthAllowed {
		submergedVol := calculateTotalSubmergedVolume(params.Buoy, currentDepth)

		surfaceWind := calculateTotalSurfaceEmergee(params.Buoy, currentDepth)
		windForce := CalculateWindDrag(conditions.WindVelocity, surfaceWind)

		vitesseWave := 0.0
		if conditions.WavePeriod > 0 {
			vitesseWave = math.Pi * (conditions.WaveHeight / 1.85) / conditions.WavePeriod
		}
		vitesseCourantSurface := conditions.CurrentVelocity + vitesseWave + (conditions.WindVelocity * 0.015)

		surfaceSubmerged := calculateTotalSurfaceImmergee(params.Buoy, currentDepth)
		currentForceBuoy := CalculateBuoyCurrentDrag(vitesseCourantSurface, surfaceSubmerged)
		currentForceChain := CalculateChainDrag(conditions.CurrentVelocity, surfaceChaineRef)

		totalHorizontalEffort := CalculateTotalHorizontalEffort(windForce, currentForceBuoy, currentForceChain)

		// C. ÉQUILIBRE CATÉNAIRE
		longueurCatenaire := math.Sqrt(CalculateCatenaryLength(hauteurCatenaire, totalHorizontalEffort, poidsLineicImmerge))
		poidsCatenaireImmergee := longueurCatenaire * poidsLineicImmerge

		poidsTotauxImmerges := masseBoueeFixe + poidsLestImmerge + poidsCatenaireImmergee
		volumeQuiDoitEtreImmerge := poidsTotauxImmerges / WaterDensity

		if submergedVol >= volumeQuiDoitEtreImmerge {
			// --- RÉSULTAT TROUVÉ ---
			result.Enfoncement = currentDepth
			result.VolumeDeplacer = submergedVol
			result.DeplacementTotal = poidsTotauxImmerges / 1000.0 // Conversion Tonnes
			result.EffortHorizontalKg = totalHorizontalEffort
			result.LongueurCatenaire = longueurCatenaire
			
			// Tension en TONNES
			result.TensionMaxMouillage = math.Sqrt(math.Pow(poidsCatenaireImmergee/1000.0, 2) + math.Pow(totalHorizontalEffort/1000.0, 2))

			// Franc-Bord
			hFlotteur := 0.0
			for _, el := range params.Buoy.FlotteurData.Elements { hFlotteur += el.Hauteur }
			result.FrancBordBouee = hFlotteur - currentDepth

			volTotal := calculateTotalVolume(params.Buoy)
			if volTotal > 0 {
				result.ReserveFlotabilite = math.Round(((volTotal - submergedVol)/volTotal)*10000) / 100
			}

			result.CoefSecChaine = CalculateChainSafetyCoefficient(params.Chain.ChargeEpreuveQ2*1000.0, hauteurCatenaire, poidsLineicImmerge, totalHorizontalEffort)
			result.MasseMinimaleCM = CalculateMinAnchorMass(totalHorizontalEffort, params.AnchorDensity/1000.0)
			result.CoefSecCM = CalculateSubmergedAnchorWeight(result.MasseMinimaleCM, params.AnchorDensity/1000.0) / totalHorizontalEffort

			result.ProfondeurMax = CalculateMaxDepth(conditions.WaterDepth, conditions.Marnage, conditions.WaveHeight)
			result.ProfondeurMin = CalculateMinDepth(conditions.WaterDepth, conditions.WaveHeight)
			result.RayonEvitage = CalculateSwingRadius(totalHorizontalEffort, poidsLineicImmerge, hauteurCatenaire)
			result.AngleTangence = CalculateTangencyAngle(poidsCatenaireImmergee, result.TensionMaxMouillage)
			result.TirantEau = CalculateDraught(currentDepth, params.Buoy.StructureData.OffsetFlotteur)

			result.TimeCalcul = time.Since(startTime)
			result.ChainType = fmt.Sprintf("%s DN%.0f", params.Chain.Type, params.Chain.DN)
			result.DiametreChaine = params.Chain.DN / 1000.0
			
			return result, nil
		}
		currentDepth += submersionStep
	}

	return result, fmt.Errorf("❌ Équilibre non trouvé (Bouée dépasse maxDepth %.1f)", maxDepthAllowed)
}

func calculateTotalSubmergedVolume(buoy models.Buoy, currentFloatDepth float64) float64 {
	vol := 0.0
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		seg := TroncConeElement{DiameterLow: el.DiametreBas, DiameterHigh: el.DiametreHaut, DiameterInter: el.DiametreInt, HauteurElement: el.Hauteur, VolumeReel: el.Volume}
		if currentFloatDepth >= hSoFar {
			immersion := math.Min(el.Hauteur, currentFloatDepth-hSoFar)
			vol += seg.VolumeByHauteur(immersion)
		}
		hSoFar += el.Hauteur
	}
	hImmergeeStructure := currentFloatDepth + buoy.StructureData.OffsetFlotteur
	hSoFar = 0.0
	for _, el := range buoy.StructureData.Elements {
		seg := TroncConeElement{DiameterLow: el.DiametreBas, DiameterHigh: el.DiametreHaut, DiameterInter: el.DiametreInt, HauteurElement: el.Hauteur, VolumeReel: el.Volume}
		if hImmergeeStructure >= hSoFar {
			immersion := math.Min(el.Hauteur, hImmergeeStructure-hSoFar)
			vol += seg.VolumeByHauteur(immersion)
		}
		hSoFar += el.Hauteur
	}
	return vol
}

func calculateTotalSurfaceImmergee(buoy models.Buoy, currentFloatDepth float64) float64 {
	surf := 0.0
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		if currentFloatDepth > hSoFar {
			immersion := math.Min(el.Hauteur, currentFloatDepth-hSoFar)
			L_Inter := el.DiametreBas + (immersion * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
			surf += immersion * (el.DiametreBas + L_Inter) / 2.0
		}
		hSoFar += el.Hauteur
	}
	hImmergeeStructure := currentFloatDepth + buoy.StructureData.OffsetFlotteur
	hSoFar = 0.0
	for _, el := range buoy.StructureData.Elements {
		if hImmergeeStructure > hSoFar {
			immersion := math.Min(el.Hauteur, hImmergeeStructure-hSoFar)
			L_Inter := el.DiametreBas + (immersion * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
			surf += immersion * (el.DiametreBas + L_Inter) / 2.0
		}
		hSoFar += el.Hauteur
	}
	return surf
}

func calculateTotalSurfaceEmergee(buoy models.Buoy, currentFloatDepth float64) float64 {
	surfFixed := (buoy.PyloneData.Height * (buoy.PyloneData.WidthHigh + buoy.PyloneData.WidthLow) / 2.0) +
		(buoy.EquipementData.Height * (buoy.EquipementData.WidthHigh + buoy.EquipementData.WidthLow) / 2.0)
	surfFloat := 0.0
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		hTotalTranche := hSoFar + el.Hauteur
		if hTotalTranche > currentFloatDepth {
			emergeDansTranche := el.Hauteur
			if currentFloatDepth > hSoFar {
				emergeDansTranche = hTotalTranche - currentFloatDepth
			}
			immersionAbsolue := math.Max(0, currentFloatDepth - hSoFar)
			L_Flottaison := el.DiametreBas + (immersionAbsolue * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
			surfFloat += emergeDansTranche * (el.DiametreHaut + L_Flottaison) / 2.0
		}
		hSoFar += el.Hauteur
	}
	return surfFixed + surfFloat
}

func calculateTotalMass(buoy models.Buoy) float64 {
	return buoy.FlotteurData.Masse + buoy.StructureData.Masse + buoy.PyloneData.Masse + buoy.EquipementData.Masse
}

func calculateTotalVolume(buoy models.Buoy) float64 {
	v := 0.0
	for _, el := range buoy.FlotteurData.Elements { v += el.Volume }
	for _, el := range buoy.StructureData.Elements { v += el.Volume }
	return v
}
