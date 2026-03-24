package calc

import (
	"backend/internal/models"
	"fmt"
	"math"
)

// SiteConditions représente les variables météo saisies par l'utilisateur
type SiteConditions struct {
	WaterDepth      float64 // Profondeur (H) en mètres
	WindVelocity    float64 // Vitesse du vent (V_wind) en m/s
	CurrentVelocity float64 // Vitesse du courant (V_current) en m/s
	WaveHeight      float64 // Hauteur de la houle (H_wave) en mètres
	WavePeriod      float64 // Période de la houle (T) en secondes
}



// FindEquilibrium fait tourner la boucle d'équilibre pas-à-pas (Simulation d'Ancrage)
func FindEquilibrium(buoy models.Buoy, conditions SiteConditions) (SimulationResult, error) {
	fmt.Printf("🔬 Simulation d'équilibre IALA pour la bouée : %s\n", buoy.Name)

	var result SimulationResult

	// --- 1. CONFIGURATION DE BASE ---
	submersionStep := 0.005 // Pas de 5 millimètres conforme VB et PHP
	currentDepth := 0.0     // On part du principe que la bouée flotte au ras de l'eau
	maxDepthAllowed := 15.0 // Sécurité anti-boucle infinie (Hauteur totale)

	// --- 1. PARAMÈTRES CATÉNAIRES PRÉ-CALCULÉS ---
	// [ATTENTE_API] : Ces constantes seront mappées depuis les tables PostgreSQL (Semaine 3)
	const massLineiqueAirExemple = 20.0 // kg/m dans l'air (Ex: Chaîne 24mm)
	poidsLineicImmerge := CalculateLineicWeight(massLineiqueAirExemple)

	hauteurCatenaire := conditions.WaterDepth // Équivalent HAUTEUR_CATENAIRE VB.NET

	// --- 2. LA BOUCLE MAGIQUE CALMAR ---
	for currentDepth < maxDepthAllowed {
		// A. Calculer le volume immergé à cette profondeur exacte
		submergedVolume := calculateTotalSubmergedVolume(buoy, currentDepth)

		// B. Force d'Archimède (pousse vers le haut en Newtons)
		buoyancy := CalculateBuoyancy(submergedVolume)

		// --- C. CALCUL DES TRAINÉES (CONFORME IALA) ---
		// 1. Force du Vent sur la partie EMERGEE (Air)
		surfaceEmergee := calculateTotalSurfaceEmergee(buoy, currentDepth)
		windForce := CalculateWindDrag(conditions.WindVelocity, surfaceEmergee)

		// 2. Vitesse Courant Surface Combinée (Tidal + wave + Vent)
		// Formule IALA VB.NET : VitesseCourant + VitesseWave + (VitesseVent * 0.015)
		vitesseWave := 0.0
		if conditions.WavePeriod > 0 {
			vitesseWave = math.Pi * (conditions.WaveHeight / 1.85) / conditions.WavePeriod
		}
		vitesseCourantSurface := conditions.CurrentVelocity + vitesseWave + (conditions.WindVelocity * 0.015)

		// 3. Force du Courant sur la Bouée Immergée (Eau)
		surfaceImmergee := calculateTotalSurfaceImmergee(buoy, currentDepth)
		currentForceBuoy := CalculateBuoyCurrentDrag(vitesseCourantSurface, surfaceImmergee)

		// 4. Force du Courant sur la Chaîne
		// Note : L'IALA demande d'évaluer la Traînée de la Chaîne à chaque itération
		surfaceChaine := hauteurCatenaire * (0.024 * 2.65) // Diametre de 24mm exemple
		currentForceChain := CalculateChainDrag(conditions.CurrentVelocity, surfaceChaine)

		// 5. Total de l'Effort Horizontal
		totalHorizontalEffort := CalculateTotalHorizontalEffort(windForce, currentForceBuoy, currentForceChain)

		// --- D. CATÉNAIRE & LONGUEUR ---
		longueurCatenaire := math.Sqrt(CalculateCatenaryLength(hauteurCatenaire, totalHorizontalEffort, poidsLineicImmerge))

		// Poids vertical de la chaîne levée par la météo
		poidsCatenaireImmergee := longueurCatenaire * poidsLineicImmerge

		// --- E. ÉQUILIBRE DES FORCES (VB.NET : VOLUME_IMMERGE >= VOLUME_DEPLACE) ---
		// Total Déplacement = MasseBouee + Équipement + PoidsCaténaire
		masseBoueeTotale := calculateTotalMass(buoy)
		deplacementTotal := masseBoueeTotale + poidsCatenaireImmergee
		volumeDeplace := deplacementTotal / WaterDensity

		// Si Archimède suffit à porter la bouée + les tonnes de chaîne soulevées
		if submergedVolume >= volumeDeplace {
			result.Enfoncement = currentDepth
			result.VolumeDeplacer = submergedVolume
			result.DeplacementTotal = deplacementTotal / 1000.0 // Conversion en Tonnes VB.NET
			result.EffortHorizontalKg = totalHorizontalEffort
			result.LongueurCatenaire = longueurCatenaire

			// Calcul de la Tension par Pythagore
			result.TensionMaxMouillage = math.Sqrt(math.Pow(poidsCatenaireImmergee/1000.0, 2) + math.Pow(totalHorizontalEffort, 2))

			// Rayon d'évitage (Swinging Radius)
			result.RayonEvitage = CalculateSwingRadius(totalHorizontalEffort, poidsLineicImmerge, hauteurCatenaire)
			break
		}

		// On s'enfonce d'un millimètre pour la suite de l'équilibre
		_ = buoyancy // On utilise submergedVolume directement pour l'équilibre volumétrique VB.NET
		currentDepth += submersionStep
	}

	// Note: result.FrancBordBouee sera calculé après la sortie de la boucle
	return result, nil
}

// --- FONCTIONS SECONDAIRES (À implémenter avec les tranches réelles) ---

func calculateTotalSubmergedVolume(buoy models.Buoy, currentDepth float64) float64 {
	totalVolume := 0.0
	heightSoFar := 0.0 // On empile les tranches de bas en haut

	// 1. Parcourir les tranches du Flotteur
	for _, item := range buoy.FlotteurData.Elements {
		// Création d'un wrapper géométrique pour bénéficier des fonctions calculées
		segment := TroncConeElement{
			DiameterLow:    item.DiametreBas,
			DiameterHigh:   item.DiametreHaut,
			DiameterInter:  item.DiametreInt,
			HauteurElement: item.Hauteur,
			VolumeReel:     item.Volume,
		}

		// Vérifier si la tranche est immergée
		if currentDepth <= heightSoFar {
			// La tranche est complètement hors d'eau
			break
		}

		if currentDepth >= (heightSoFar + item.Hauteur) {
			// La tranche est complètement sous l'eau
			totalVolume += segment.GetVolume()
		} else {
			// La tranche est partiellement immergée (au niveau de la flottaison)
			immersionDansTranche := currentDepth - heightSoFar
			totalVolume += segment.VolumeByHauteur(immersionDansTranche)
		}

		heightSoFar += item.Hauteur
	}

	return totalVolume
}

// calculateTotalMass additionne toutes les masses des composants (Mètres)
func calculateTotalMass(buoy models.Buoy) float64 {
	totalMass := 0.0
	totalMass += buoy.FlotteurData.Masse
	totalMass += buoy.StructureData.Masse
	totalMass += buoy.PyloneData.Masse
	totalMass += buoy.EquipementData.Masse
	return totalMass
}

// calculateTotalSurfaceEmergee calcule la surface au vent de la bouée
func calculateTotalSurfaceEmergee(buoy models.Buoy, currentDepth float64) float64 {
	// 1. Surfaces fixes au dessus
	surfaceSuperstructure := 0.0
	surfaceSuperstructure += (buoy.PyloneData.Height * (buoy.PyloneData.WidthHigh + buoy.PyloneData.WidthLow) / 2.0)
	surfaceSuperstructure += (buoy.EquipementData.Height * (buoy.EquipementData.WidthHigh + buoy.EquipementData.WidthLow) / 2.0)

	// 2. Surface Variable du flotteur (partie émergée)
	surfaceFlotteurEmergee := 0.0
	heightSoFar := 0.0

	for _, item := range buoy.FlotteurData.Elements {
		if currentDepth <= heightSoFar {
			// Complètement émergé (au dessus de la flottaison)
			// Calcul Surface Trapèze standard
			surfaceFlotteurEmergee += item.Hauteur * (item.DiametreHaut + item.DiametreBas) / 2.0
		} else if currentDepth < (heightSoFar + item.Hauteur) {
			// Partiellement émergé (la ligne de flottaison coupe le cône)
			immersionDansTranche := currentDepth - heightSoFar
			hauteurRestante := item.Hauteur - immersionDansTranche

			// Diamètre intermédiaire à la ligne de flottaison
			L_Inter := item.DiametreBas + (immersionDansTranche * (item.DiametreHaut - item.DiametreBas) / item.Hauteur)

			surfaceFlotteurEmergee += hauteurRestante * (item.DiametreHaut + L_Inter) / 2.0
		}
		// Si currentDepth >= heightSoFar + item.Hauteur, la tranche est 100% sous l'eau (Surface émergée = 0)

		heightSoFar += item.Hauteur
	}

	return surfaceSuperstructure + surfaceFlotteurEmergee
}

// calculateTotalSurfaceImmergee calcule la surface immergée soumise au courant (Mètres)
func calculateTotalSurfaceImmergee(buoy models.Buoy, currentDepth float64) float64 {
	surfaceImmergee := 0.0
	heightSoFar := 0.0

	for _, item := range buoy.FlotteurData.Elements {
		if currentDepth <= heightSoFar {
			// La tranche est hors d'eau
			break
		}

		if currentDepth >= (heightSoFar + item.Hauteur) {
			// Complètement immergé (Calcul Surface Trapèze standard)
			surfaceImmergee += item.Hauteur * (item.DiametreHaut + item.DiametreBas) / 2.0
		} else {
			// Partiellement immergé (la ligne de flottaison coupe le cône)
			immersionDansTranche := currentDepth - heightSoFar

			// Diamètre intermédiaire à la ligne de flottaison
			L_Inter := item.DiametreBas + (immersionDansTranche * (item.DiametreHaut - item.DiametreBas) / item.Hauteur)

			surfaceImmergee += immersionDansTranche * (item.DiametreBas + L_Inter) / 2.0
		}

		heightSoFar += item.Hauteur
	}

	return surfaceImmergee
}

func calculateChainTension(buoy models.Buoy, depth float64) float64 {
	// PLUS TARD : Formule de la Caténaire classique
	// Tension = Poids_Linaire_Chaîne * Facteur_Profondeur
	return depth * 40.0 * 9.81 // Poids factice pour le test
}
