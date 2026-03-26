package calc

import (
	"backend/internal/models"
	"fmt"
	"math"
	"time"
)


// FindEquilibrium fait tourner la boucle d'équilibre pas-à-pas
func FindEquilibrium(params SimulationParams, conditions SiteConditions) (SimulationResult, error) {
	fmt.Printf("🔬 Simulation IALA : %s (Chaîne: %s DN%.1f)\n",
		params.Buoy.Name, params.Chain.Type, params.Chain.DN)

	startTime := time.Now()
	var result SimulationResult

	// 1. Initialisation des paramètres de site
	waterDensity := conditions.WaterDensity
	if waterDensity <= 0 {
		waterDensity = WaterDensity // Valeur par défaut (1025.0)
	}
	submersionStep := 0.005
	currentDepth := 0.0
	// Hauteur totale du flotteur
	maxDepthAllowed := GetComponentTotalHeight(params.Buoy.FlotteurData)

	poidsLineicImmerge := params.Chain.MasseLineique * (1.0 - (waterDensity / MetalDensity))
	diametreChaineM := params.Chain.DN / 1000.0

	// 2. Masses Fixes
	masseBoueeFixe := calculateTotalMass(params.Buoy) + params.ExtraEquipmentMass
	masseLestTotale := float64(params.NumBallast) * params.Buoy.MasseLestUnitaire
	poidsLestImmerge := CalculateSubmergedBallastWeightDynamic(masseLestTotale, waterDensity)

	// 3. Boucle d'Équilibre
	for currentDepth <= maxDepthAllowed {
		submergedVol := calculateTotalSubmergedVolume(params.Buoy, currentDepth)
		
		// Distance verticale réelle Orgueil/Organeau <-> Fond
		hauteurCatenaire := conditions.WaterDepth - (currentDepth + params.Buoy.StructureData.OffsetOrganeau)
		if hauteurCatenaire <= 0 {
			hauteurCatenaire = 0.001
		}

		surfaceChaineRef := hauteurCatenaire * (diametreChaineM * 2.65)
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
		longueurCatenaireSq := CalculateCatenaryLength(hauteurCatenaire, totalHorizontalEffort, poidsLineicImmerge)
		longueurCatenaire := math.Sqrt(longueurCatenaireSq)
		poidsCatenaireImmergee := longueurCatenaire * poidsLineicImmerge

		poidsTotauxImmerges := masseBoueeFixe + poidsLestImmerge + poidsCatenaireImmergee
		volumeQuiDoitEtreImmerge := poidsTotauxImmerges / waterDensity

		if submergedVol >= volumeQuiDoitEtreImmerge {
			// --- RÉSULTAT TROUVÉ ---
			result.Enfoncement = currentDepth
			result.TensionMaxMouillage = math.Sqrt(math.Pow(poidsCatenaireImmergee/1000.0, 2) + math.Pow(totalHorizontalEffort/1000.0, 2))
			result.RayonEvitage = CalculateSwingRadius(totalHorizontalEffort, poidsLineicImmerge, hauteurCatenaire)
			result.FrancBord = maxDepthAllowed - currentDepth
			result.TirantEau = currentDepth + params.Buoy.StructureData.OffsetFlotteur
			
			// Calcul de la réserve de flottabilité (%)
			volTotalFlotteur := 0.0
			for _, el := range params.Buoy.FlotteurData.Elements { volTotalFlotteur += el.Volume }
			if volTotalFlotteur > 0 {
				result.ReserveFlottabilite = ((volTotalFlotteur - submergedVol) / volTotalFlotteur) * 100.0
			}
			
			result.TraineeVent = windForce
			result.TraineeCourant = currentForceChain
			result.TraineeVague = currentForceBuoy
			result.VitesseCourantSurface = vitesseCourantSurface
			
			// Sécurité IALA
			result.IsSafe = true
			result.CoefSecuriteChaine = CalculateChainSafetyCoefficient(params.ChainQualityCharge, hauteurCatenaire, poidsLineicImmerge, totalHorizontalEffort)
			result.MasseMinCorpsMort = CalculateMinAnchorMass(totalHorizontalEffort, params.AnchorDensity) 

			result.BuoyName = params.Buoy.Name
			result.ChainType = params.Chain.Type
			result.SiteConditions = conditions
			
			result.TimeCalcul = float64(time.Since(startTime).Milliseconds())
			return result, nil
		}

		currentDepth += submersionStep
	}

	return result, fmt.Errorf("équilibre non trouvé (bouée coule ou profondeur insuffisante)")
}

func calculateTotalMass(buoy models.Buoy) float64 {
	mass := buoy.FlotteurData.Masse + buoy.StructureData.Masse
	for _, p := range buoy.PyloneData {
		mass += p.Masse
	}
	for _, e := range buoy.EquipementData {
		mass += e.Masse
	}
	return mass
}

func calculateTotalSubmergedVolume(buoy models.Buoy, immersion float64) float64 {
	vol := CalculateComponentSubmergedVolume(buoy.FlotteurData, immersion)
	// Structure offset (OffsetFlotteur est la position relative au bas du flotteur)
	structImmersion := immersion + buoy.StructureData.OffsetFlotteur
	vol += CalculateComponentSubmergedVolume(buoy.StructureData, structImmersion)
	return vol
}

func calculateTotalSurfaceEmergee(buoy models.Buoy, immersion float64) float64 {
	surf := CalculateComponentSurfaceEmergee(buoy.FlotteurData, immersion)
	// Surface pylônes (Somme des trapèzes)
	for _, p := range buoy.PyloneData {
		surf += p.Height * (p.WidthHigh + p.WidthLow) / 2.0
	}
	return surf
}

func calculateTotalSurfaceImmergee(buoy models.Buoy, immersion float64) float64 {
	surf := CalculateComponentSurfaceImmergee(buoy.FlotteurData, immersion)
	structImmersion := immersion + buoy.StructureData.OffsetFlotteur
	surf += CalculateComponentSurfaceImmergee(buoy.StructureData, structImmersion)
	return surf
}
