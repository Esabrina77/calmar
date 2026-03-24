package calc

import (
	"backend/internal/models"
	"fmt"
)

// SiteConditions représente les variables météo saisies par l'utilisateur
type SiteConditions struct {
	WaterDepth      float64 // Profondeur (H) en mètres
	WindVelocity    float64 // Vitesse du vent (V_wind) en m/s
	CurrentVelocity float64 // Vitesse du courant (V_current) en m/s
	WaveHeight      float64 // Hauteur de la houle (H_wave) en mètres (Optionnel pour l'instant)
}

// EquilibriumResult contient les conclusions de la simulation pour UNE bouée
type EquilibriumResult struct {
	Compatible    bool    // La bouée est-elle éligible ?
	Submersion    float64 // Enfoncement final calculé (mètres)
	Tension       float64 // Tension sur la ligne d'ancrage (Newtons)
	SafetyMargin  float64 // Marge avant submersion critique (%)
	FailureReason string  // Pourquoi elle a coulé (si Compatible = false)
}

// FindEquilibrium fait tourner la boucle d'équilibre pas-à-pas (Amarrage)
func FindEquilibrium(buoy models.Buoy, conditions SiteConditions) (EquilibriumResult, error) {
	fmt.Printf("🔬 Simulation d'équilibre pour la bouée : %s\n", buoy.Name)

	var result EquilibriumResult
	result.Compatible = false

	// --- 1. CONFIGURATION DE BASE ---
	submersionStep := 0.005 // Pas de 5 millimètres
	currentDepth := 0.0     // On part du principe que la bouée flotte au ras de l'eau
	maxDepthAllowed := 10.0 // Sécurité anti-boucle infinie (Hauteur totale)

	// coefficients standards par défaut
	cdWind := 1.0
	cdCurrent := 1.2

	// --- 2. LA BOUCLE MAGIQUE CALMAR ---
	for currentDepth < maxDepthAllowed {
		// A. Calculer le volume immergé à cette profondeur exacte
		// (On utilisera nos fonctions physics.go sur les tranches StructureData/FlotteurData)
		submergedVolume := calculateTotalSubmergedVolume(buoy, currentDepth)

		// B. Calculer la force d'Archimède (pousse vers le haut)
		buoyancy := CalculateBuoyancy(submergedVolume)

		// C. Calculer les Traînées de la météo (poussent sur le coté et tirent)
		// TODO: On doit calculer les SURFACES projetées au vent et au courant
		windForce := CalculateWindDrag(conditions.WindVelocity, 2.0 /*Surface exposee*/, cdWind)
		currentForce := CalculateCurrentDrag(conditions.CurrentVelocity, 3.0 /*Surface immergee*/, cdCurrent)

		// D. Calculer la Tension de la Chaîne (poids du "Mou")
		// Dans la théorie des caténaires : La chaîne pèse lourd.
		chainTension := calculateChainTension(buoy, conditions.WaterDepth)

		// E. Vérification de l'équilibre des forces
		// Forces_Up (Archimède) = Forces_Down (Poids + Tension_Verticale + Effort)
		if buoyancy >= (chainTension + windForce + currentForce) {
			result.Compatible = true
			result.Submersion = currentDepth
			result.Tension = chainTension + windForce + currentForce
			break
		}

		// On s'enfonce de 5 mm
		currentDepth += submersionStep
	}

	if !result.Compatible {
		result.FailureReason = "La bouée sombre (Submersion maximale dépassée)"
	}

	return result, nil
}

// --- FONCTIONS SECONDAIRES (À implémenter avec les tranches réelles) ---

func calculateTotalSubmergedVolume(buoy models.Buoy, currentDepth float64) float64 {
	// PLUS TARD : Boucler sur buoy.StructureData.Elements et additionner
	// TruncatedConeVolume(h, d0, d1, di) pour la partie < currentDepth
	return currentDepth * 1.5 // Multiplicateur factice pour le test
}

func calculateChainTension(buoy models.Buoy, depth float64) float64 {
	// PLUS TARD : Formule de la Caténaire classique
	// Tension = Poids_Linaire_Chaîne * Facteur_Profondeur
	return depth * 40.0 * 9.81 // Poids factice pour le test
}
