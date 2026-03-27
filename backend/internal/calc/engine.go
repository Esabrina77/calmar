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

	// --- CORRECTION #1 : HAUTEUR_CATENAIRE (Norme IALA / .NET) ---
	// Alignement sur la formule .NET :
	//   HAUTEUR_CATENAIRE = PROFONDEUR_MAX + PROFONDEUR_SOUS_ORGANEAU
	//   PROFONDEUR_MAX = Profondeur + Marnage + HouleMax / 2
	//   HouleMax = WaveHeight * 1.85  (houle significative → houle maximale)
	// Cette hauteur est FIXE (pire cas = marée haute + houle max) et ne change pas en boucle
	houleMax := conditions.WaveHeight * 1.85
	profondeurMax := conditions.WaterDepth + conditions.Marnage + houleMax/2.0
	hauteurCatenaireFixe := profondeurMax + params.Buoy.StructureData.OffsetOrganeau
	if hauteurCatenaireFixe <= 0 {
		hauteurCatenaireFixe = 0.001
	}

	// --- CORRECTION #3 : Vitesse courant de surface (conforme .NET) ---
	// .NET : VITESSE_COURANT_SURFACE = _VitesseCourant + VITESSE_HOULE + VitesseVent * 0.015
	// VITESSE_HOULE = π × (HouleMax/1.85) / Période = π × WaveHeight / Période (houle signif)
	vitesseWaveFixe := 0.0
	if conditions.WavePeriod > 0 {
		vitesseWaveFixe = math.Pi * conditions.WaveHeight / conditions.WavePeriod
	}
	vitesseCourantSurface := conditions.CurrentVelocity + vitesseWaveFixe + (conditions.WindVelocity * 0.015)

	// Surface chaîne basée sur hauteur caténaire fixe (IALA)
	surfaceChaineRef := hauteurCatenaireFixe * (diametreChaineM * 2.65)
	currentForceChain := CalculateChainDrag(conditions.CurrentVelocity, surfaceChaineRef, waterDensity)

	// 3. Boucle d'Équilibre (seul l'enfoncement varie)
	for currentDepth <= maxDepthAllowed {
		submergedVol := calculateTotalSubmergedVolume(params.Buoy, currentDepth)

		surfaceWind := calculateTotalSurfaceEmergee(params.Buoy, currentDepth)
		windForce := CalculateWindDrag(conditions.WindVelocity, surfaceWind)

		surfaceSubmerged := calculateTotalSurfaceImmergee(params.Buoy, currentDepth)
		currentForceBuoy := CalculateBuoyCurrentDrag(vitesseCourantSurface, surfaceSubmerged, waterDensity)

		totalHorizontalEffort := CalculateTotalHorizontalEffort(windForce, currentForceBuoy, currentForceChain)

		// Caténaire avec hauteur FIXE (IALA)
		hauteurCatenaire := hauteurCatenaireFixe
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
			
			// Nouveaux champs pour Fidélité 100%
			result.LongueurCatenary = longueurCatenaire
			result.SurfaceLateraleEmergee = surfaceWind
			result.SurfaceLateraleImmergee = surfaceSubmerged
			result.EffortHorizontalKg = totalHorizontalEffort
			result.PoidsLineiqueImmerge = poidsLineicImmerge
			result.PoidsLestImmerge = poidsLestImmerge
			result.MasseBoueeFixe = masseBoueeFixe

			// Calcul du volume total disponible nominal (Fidélité 100%)
			// On inclut Flotteur + Structure + Pylônes + Équipements
			volTotalTotal := 0.0
			
			// Helper pour sommer le volume réel ou théorique
			sumVol := func(comp models.ComponentData) float64 {
				v := 0.0
				for _, el := range comp.Elements {
					if el.Volume > 0 {
						v += el.Volume
					} else {
						// Calcul théorique net
						b1 := math.Pi * math.Pow(el.DiametreBas/2, 2)
						b2 := math.Pi * math.Pow(el.DiametreHaut/2, 2)
						vTotal := (el.Hauteur / 3.0) * (b1 + math.Sqrt(b1*b2) + b2)
						vCreux := math.Pi * math.Pow(el.DiametreInt/2, 2) * el.Hauteur
						v += (vTotal - vCreux)
					}
				}
				return v
			}

			volTotalTotal += sumVol(params.Buoy.FlotteurData)
			volTotalTotal += sumVol(params.Buoy.StructureData)
			
			// Ajout du volume des pylônes (approximés par des cylindres/troncs de cône)
			for _, p := range params.Buoy.PyloneData {
				b1 := math.Pi * math.Pow(p.WidthLow/2, 2)
				b2 := math.Pi * math.Pow(p.WidthHigh/2, 2)
				volTotalTotal += (p.Height / 3.0) * (b1 + math.Sqrt(b1*b2) + b2)
			}
			for _, e := range params.Buoy.EquipementData {
				b1 := math.Pi * math.Pow(e.WidthLow/2, 2)
				b2 := math.Pi * math.Pow(e.WidthHigh/2, 2)
				volTotalTotal += (e.Height / 3.0) * (b1 + math.Sqrt(b1*b2) + b2)
			}

			if volTotalTotal > 0 {
				result.ReserveFlottabilite = ((volTotalTotal - submergedVol) / volTotalTotal) * 100.0
			}
			
			result.TraineeVent = windForce
			result.TraineeCourant = currentForceChain
			result.TraineeVague = currentForceBuoy
			result.VitesseCourantSurface = vitesseCourantSurface
			
			// Alignement Poids Total : .NET semble inclure 10% de masse supplémentaire (Visserie/Sécurité)
			// sur la masse à l'équilibre pour beaucoup de modèles.
			result.MasseBoueeFixe = poidsTotauxImmerges 
			
			// Sécurité IALA & Flottabilité
			result.CoefSecuriteChaine = CalculateChainSafetyCoefficient(params.ChainQualityCharge, hauteurCatenaire, poidsLineicImmerge, totalHorizontalEffort)
			result.MasseMinCorpsMort = CalculateMinAnchorMass(totalHorizontalEffort, params.AnchorDensity, conditions.WaterDensity) 
			
			// Un résultat est considéré comme sûr si :
			// 1. La réserve de flottabilité est positive (norme IALA suggère souvent > 20%, on met 15% en seuil critique)
			// 2. Le franc-bord est positif
			// 3. Le coefficient de sécurité de la chaîne est >= 5 (IALA)
			result.IsSafe = result.ReserveFlottabilite > 15.0 && result.FrancBord > 0.1 && result.CoefSecuriteChaine >= 5.0

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
	for _, e := range buoy.EquipementData {
		surf += e.Height * (e.WidthHigh + e.WidthLow) / 2.0
	}
	return surf
}

func calculateTotalSurfaceImmergee(buoy models.Buoy, immersion float64) float64 {
	surf := CalculateComponentSurfaceImmergee(buoy.FlotteurData, immersion)
	structImmersion := immersion + buoy.StructureData.OffsetFlotteur
	surf += CalculateComponentSurfaceImmergee(buoy.StructureData, structImmersion)
	return surf
}
