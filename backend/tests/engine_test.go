package tests

import (
	"log"
	"testing"
	"backend/internal/calc"
	"backend/internal/models"
)

func TestEngineLogicWithMockData(t *testing.T) {
	// 1. MOCK BUOY (Cube de 1x1x1.2m)
	// On crée un composant flotteur avec une tranche cubique
	mockFlotteur := models.ComponentData{
		Name:  "MOCK_CUBE",
		Masse: 500.0, // kg
		Elements: []models.ElementDimItem{
			{Hauteur: 1.2, DiametreBas: 1.128, DiametreHaut: 1.128, DiametreInt: 0}, // Surface approx 1m2
		},
	}

	mockBuoy := models.Buoy{
		Name:          "MOCK_CUBE_B800",
		FlotteurData:  mockFlotteur,
		StructureData: models.ComponentData{Masse: 0}, // Pas de structure pour le mock
	}

	// 2. MOCK CHAIN
	mockChain := models.Chain{
		Type:           "CHAIN_TEST",
		DN:             14.0,
		MasseLineique:  4.4, // kg/m
		ChargeEpreuveQ1: 120.0,
	}

	// 3. PARAMÈTRES SIMULATION
	params := calc.SimulationParams{
		Buoy:          mockBuoy,
		Chain:         mockChain,
		NumBallast:    0,
		LestDensity:   7320.0,
		AnchorDensity: 2400.0,
	}

	// 4. CONDITIONS CALMES (Pour valider Archimède pure)
	conditions := calc.SiteConditions{
		WaterDepth:      10.0,
		WaterDensity:    1025.0,
		WindVelocity:    0.0,
		CurrentVelocity: 0.0,
		WaveHeight:      0.0,
	}

	// 5. RUN ENGINE
	result, err := calc.FindEquilibrium(params, conditions)
	if err != nil {
		t.Fatalf("❌ Erreur critique moteur : %v", err)
	}

	// 6. VALIDATION LOGIQUE
	log.Printf("📊 MOCK Cube Equilibre - Enfoncement=%.3fm, Tension=%.2ft, Franc-Bord=%.2fm", 
		result.Enfoncement, result.TensionMaxMouillage, result.FrancBord)

	if result.Enfoncement < 0.4 || result.Enfoncement > 0.6 {
		t.Errorf("❌ Enfoncement attendu autour de ~0.5m, got %f", result.Enfoncement)
	}
}
