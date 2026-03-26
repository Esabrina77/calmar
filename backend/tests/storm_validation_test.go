package tests

import (
	"backend/internal/calc"
	"backend/internal/models"
	"fmt"
	"testing"
)

func TestStormConditionsQ3Steel(t *testing.T) {
	// Simulations d'une bouée JET 2500
	buoy := models.Buoy{
		Name: "JET 2500",
		StructureData: models.ComponentData{
			OffsetOrganeau: 4.5,
			OffsetFlotteur: 1.2,
		},
		FlotteurData: models.ComponentData{
			Elements: []models.ElementDimItem{
				{Hauteur: 2.0, DiametreBas: 1.8, DiametreHaut: 1.8, Volume: 2.5}, // 2500 Litres
			},
		},
		MasseLestUnitaire: 25.0,
	}
	
	// Chaîne DN 32
	chain := models.Chain{
		Type: "3D",
		DN: 32,
		ChargeEpreuveQ1: 45.0,
		ChargeEpreuveQ2: 65.0,
		ChargeEpreuveQ3: 90.0, // Q3
	}

	params := calc.SimulationParams{
		Buoy:               buoy,
		Chain:              chain,
		NumBallast:         12,      // Lestage lourd
		ExtraEquipmentMass: 150.0,   // AIS + Batteries + Capteurs
		LestDensity:        7850.0,  // Fonte
		AnchorDensity:      7850.0,  // ACY (Acier)
		ChainQualityCharge: 90.0,    // Force Q3
	}

	conditions := calc.SiteConditions{
		WaterDepth:      25.0,
		WaterDensity:    1025.0,
		Marnage:         5.0,
		WindVelocity:    20.0,   // Vent fort mais raisonnable
		CurrentVelocity: 1.0,    // 2 noeuds
		WaveHeight:      1.5,    // Houle standard
		WavePeriod:      6.0,
	}

	result, err := calc.FindEquilibrium(params, conditions)
	if err != nil {
		t.Fatalf("Erreur simulation : %v", err)
	}

	fmt.Printf("\n🌪️ --- RÉSULTATS TEMPÊTE (Q3 + ACIER) ---\n")
	fmt.Printf("⚓ Enfoncement : %.2f m\n", result.Enfoncement)
	fmt.Printf("💪 Tension Max : %.2f t\n", result.TensionMaxMouillage)
	fmt.Printf("🛡️ Coef Sécurité : %.2f (Cible IALA > 2.0)\n", result.CoefSecuriteChaine)
	fmt.Printf("🏗️ Masse Ancre Min : %.0f kg\n", result.MasseMinCorpsMort)
	fmt.Printf("------------------------------------------\n")

	if result.CoefSecuriteChaine < 2.0 {
		t.Log("⚠️ Attention : Coefficient de sécurité faible pour ces conditions !")
	}
}
