package tests

import (
	"log"
	"testing"
	"backend/internal/calc"
	"backend/internal/models"
)

// TestEngineLogicWithMockData valide la logique interne de l'algorithme d'équilibre
// sans se connecter à PostgreSQL (MOCK).
func TestEngineLogicWithMockData(t *testing.T) {
	
	// 1. MOCK d'une bouée "Cube" simple (1m x 1m x 1m)
	// Base=1.0, Haut=1.0, H=1.0 -> Volume = 1m3
	buoyMock := models.Buoy{
		Name: "MOCK_CUBE_B800",
		FlotteurData: models.ComponentData{
			Masse: 500.0, // 500kg (Masse fixe de la bouée)
			Elements: []models.ElementDimItem{
				{Hauteur: 1.0, DiametreBas: 1.0, DiametreHaut: 1.0, Volume: 1.0},
			},
		},
		StructureData: models.ComponentData{
			Masse: 0,
			OffsetFlotteur: 0,
		},
	}

	// 2. MOCK d'une chaîne
	chainMock := models.Chain{
		Type: "CHAIN_TEST",
		DN: 14,
		MasseLineique: 4.3, // kg/m
		ChargeEpreuveQ2: 100.0, // tonnes
	}

	// 3. Paramètres
	params := calc.SimulationParams{
		Buoy:          buoyMock,
		Chain:         chainMock,
		NumBallast:    0,
		LestDensity:   7320.0,
		AnchorDensity: 2400.0,
	}

	// 4. Conditions Calmes (Équilibre Statique)
	site := calc.SiteConditions{
		WaterDepth:      10.0, // m
		WindVelocity:    0.0,  // Pas d'effort horizontal
		CurrentVelocity: 0.0,
		WaveHeight:      0.0,
	}

	// 5. Calcul
	result, err := calc.FindEquilibrium(params, site)
	if err != nil {
		t.Fatalf("❌ Erreur de calcul : %v", err)
	}

	// 6. VALIDATION LOGIQUE
	// À l'équilibre statique (sans vent/courant) :
	// Poids Total = MasseBouee (500kg) + PoidsChaîneLevée(?)
	// Pour un cube de 1m3 (1025kg/m3 d'eau)
	// Si immersion est h, volume = h m3. Archimède = 1025 * h.
	// Si h = 0.5, Archimède = 512.5 kg.
	// Donc h doit être un peu supérieur à 0.5 (car y'a le petit poids de la chaîne).
	
	log.Printf("📊 MOCK Cube Equilibre - Enfoncement=%.3fm, Tension=%.2ft, Franc-Bord=%.2fm", 
		result.Enfoncement, result.TensionMaxMouillage, result.FrancBordBouee)

	if result.Enfoncement < 0.4 || result.Enfoncement > 0.6 {
		t.Errorf("❌ Enfoncement attendu autour de ~0.5m, got %f", result.Enfoncement)
	}

	if result.FrancBordBouee < 0.4 || result.FrancBordBouee > 0.6 {
		t.Errorf("❌ Franc-Bord attendu autour de ~0.5m, got %f", result.FrancBordBouee)
	}
}
