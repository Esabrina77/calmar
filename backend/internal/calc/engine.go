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

// SimulationParams regroupe les choix de configuration de l'utilisateur pour un calcul donné
type SimulationParams struct {
	Buoy        models.Buoy
	Chain       models.Chain
	NumBallast  int     // Nombre de lests choisis
	LestDensity float64 // Densité du lest (ex: 7.0 pour fonte, 2.4 pour béton)
	AnchorDensity float64 // Densité du corps mort (ex: 2.4 pour béton)
}

// FindEquilibrium fait tourner la boucle d'équilibre pas-à-pas (Simulation d'Ancrage)
func FindEquilibrium(params SimulationParams, conditions SiteConditions) (SimulationResult, error) {
	fmt.Printf("🔬 Simulation d'équilibre IALA pour la bouée : %s (Chaîne: %s DN%.1f)\n", 
		params.Buoy.Name, params.Chain.Type, params.Chain.DN)

	startTime := time.Now()
	var result SimulationResult

	// --- 1. CONFIGURATION DE BASE ---
	submersionStep := 0.005 // Pas de 5 millimètres conforme VB et PHP
	currentDepth := 0.0     // Enfoncement du FLOTTEUR (m)
	maxDepthAllowed := 20.0 // Sécurité anti-boucle infinie

	// --- 2. PARAMÈTRES CATÉNAIRES DYNAMIQUES ---
	poidsLineicImmerge := CalculateLineicWeight(params.Chain.MasseLineique)
	hauteurCatenaire := conditions.WaterDepth 

	// Rayon de la chaîne pour calcul de traînée (approximation cylindrique IALA)
	// DN est en mm, on convertit en m. On multiplie par un facteur de forme (souvent 2.65 pour Studless)
	const chainDragFactor = 2.65
	diametreChaineM := params.Chain.DN / 1000.0
	surfaceChaineRef := hauteurCatenaire * (diametreChaineM * chainDragFactor)

	// --- 3. CALCUL DES MASSES FIXES ---
	masseBoueeFixe := calculateTotalMass(params.Buoy)
	masseLestTotale := float64(params.NumBallast) * params.Buoy.MasseLestUnitaire
	
	// Poids du lest dans l'eau
	poidsLestImmerge := 0.0
	if params.LestDensity > WaterDensity {
		poidsLestImmerge = masseLestTotale * (1.0 - (WaterDensity / params.LestDensity))
	}

	// --- 4. LA BOUCLE PROPRE "CALMAR" ---
	for currentDepth < maxDepthAllowed {
		// A. Calculer le volume immergé Total (Stack Flotteur + Structure)
		submergedVol := calculateTotalSubmergedVolume(params.Buoy, currentDepth)

		// B. Calcul des efforts du site (Vent, Courant)
		surfaceWind := calculateTotalSurfaceEmergee(params.Buoy, currentDepth)
		windForce := CalculateWindDrag(conditions.WindVelocity, surfaceWind)

		// Vitesse courant combinée (IALA)
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

		// D. CONDITION D'ÉQUILIBRE (Archimède vs Poids Totaux)
		// Volume Déplacé = (MasseFixe + PoidsLestImmergé + PoidsChaîneLevée) / DensitéEau
		poidsTotauxImmerges := masseBoueeFixe + poidsLestImmerge + poidsCatenaireImmergee
		volumeQuiDoitEtreImmerge := poidsTotauxImmerges / WaterDensity

		if submergedVol >= volumeQuiDoitEtreImmerge {
			// --- RÉSULTAT TROUVÉ ---
			result.Enfoncement = currentDepth
			result.VolumeDeplacer = submergedVol
			result.DeplacementTotal = poidsTotauxImmerges / 1000.0 // t
			result.EffortHorizontalKg = totalHorizontalEffort
			result.LongueurCatenaire = longueurCatenaire
			
			// Tension (T2 = H2 + V2)
			result.TensionMaxMouillage = math.Sqrt(math.Pow(poidsCatenaireImmergee/1000.0, 2) + math.Pow(totalHorizontalEffort, 2))

			// Franc-Bord
			hFlotteur := 0.0
			for _, el := range params.Buoy.FlotteurData.Elements { hFlotteur += el.Hauteur }
			result.FrancBordBouee = hFlotteur - currentDepth

			// Réserve Flottabilité
			volTotal := calculateTotalVolume(params.Buoy)
			if volTotal > 0 {
				result.ReserveFlotabilite = math.Round(((volTotal - submergedVol)/volTotal)*10000) / 100
			}

			// Coefficients de Sécurité
			result.CoefSecChaine = CalculateChainSafetyCoefficient(params.Chain.ChargeEpreuveQ2*1000.0, hauteurCatenaire, poidsLineicImmerge, totalHorizontalEffort)
			
			// Corps Mort
			result.MasseMinimaleCM = CalculateMinAnchorMass(totalHorizontalEffort, params.AnchorDensity)
			result.CoefSecCM = CalculateSubmergedAnchorWeight(result.MasseMinimaleCM, params.AnchorDensity) / totalHorizontalEffort

			// Données du site
			result.ProfondeurMax = CalculateMaxDepth(conditions.WaterDepth, conditions.Marnage, conditions.WaveHeight)
			result.ProfondeurMin = CalculateMinDepth(conditions.WaterDepth, conditions.WaveHeight)
			result.RayonEvitage = CalculateSwingRadius(totalHorizontalEffort, poidsLineicImmerge, hauteurCatenaire)
			result.AngleTangence = CalculateTangencyAngle(poidsCatenaireImmergee, result.TensionMaxMouillage)
			result.TirantEau = CalculateDraught(currentDepth, params.Buoy.StructureData.OffsetFlotteur)

			// Métadonnées
			result.TimeCalcul = time.Since(startTime)
			result.ChainType = fmt.Sprintf("%s DN%.0f", params.Chain.Type, params.Chain.DN)
			result.DiametreChaine = params.Chain.DN / 1000.0
			
			break
		}

		currentDepth += submersionStep
	}

	return result, nil
}

// --- LOGIQUE GÉOMÉTRIQUE DE STACKING ---

func calculateTotalSubmergedVolume(buoy models.Buoy, currentFloatDepth float64) float64 {
	vol := 0.0

	// 1. Volume du Flotteur
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		seg := TroncConeElement{DiameterLow: el.DiametreBas, DiameterHigh: el.DiametreHaut, DiameterInter: el.DiametreInt, HauteurElement: el.Hauteur, VolumeReel: el.Volume}
		if currentFloatDepth > hSoFar {
			immersion := math.Min(el.Hauteur, currentFloatDepth-hSoFar)
			vol += seg.VolumeByHauteur(immersion)
		}
		hSoFar += el.Hauteur
	}

	// 2. Volume de la Structure (Offset par rapport au bas du flotteur)
	// draughtStructure = currentFloatDepth + OffsetFlotteur
	hImmergeeStructure := currentFloatDepth + buoy.StructureData.OffsetFlotteur
	hSoFar = 0.0
	for _, el := range buoy.StructureData.Elements {
		seg := TroncConeElement{DiameterLow: el.DiametreBas, DiameterHigh: el.DiametreHaut, DiameterInter: el.DiametreInt, HauteurElement: el.Hauteur, VolumeReel: el.Volume}
		if hImmergeeStructure > hSoFar {
			immersion := math.Min(el.Hauteur, hImmergeeStructure-hSoFar)
			vol += seg.VolumeByHauteur(immersion)
		}
		hSoFar += el.Hauteur
	}

	return vol
}

func calculateTotalSurfaceImmergee(buoy models.Buoy, currentFloatDepth float64) float64 {
	surf := 0.0

	// 1. Surface Flotteur
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		if currentFloatDepth > hSoFar {
			immersion := math.Min(el.Hauteur, currentFloatDepth-hSoFar)
			L_Inter := el.DiametreBas + (immersion * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
			surf += immersion * (el.DiametreBas + L_Inter) / 2.0
		}
		hSoFar += el.Hauteur
	}

	// 2. Surface Structure
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
	// Superstructure (Pylone + Equipement) : Toujours émergée par définition
	surfFixed := (buoy.PyloneData.Height * (buoy.PyloneData.WidthHigh + buoy.PyloneData.WidthLow) / 2.0) +
		(buoy.EquipementData.Height * (buoy.EquipementData.WidthHigh + buoy.EquipementData.WidthLow) / 2.0)

	// Flotteur émergé
	surfFloat := 0.0
	hSoFar := 0.0
	for _, el := range buoy.FlotteurData.Elements {
		hTotalTranche := hSoFar + el.Hauteur
		if hTotalTranche > currentFloatDepth {
			// Y'a du rab au dessus de l'eau
			emergeDansTranche := el.Hauteur
			if currentFloatDepth > hSoFar {
				emergeDansTranche = hTotalTranche - currentFloatDepth
			}
			// Diamètre à la flottaison
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
