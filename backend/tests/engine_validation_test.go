package tests

import (
	"log"
	"testing"

	"backend/internal/calc"
	"backend/internal/db"
	"backend/internal/models"

	"github.com/joho/godotenv"
)

// TestEngineWithRealDatabase valide le moteur de calcul en utilisant 
// les vraies données injectées dans PostgreSQL (Bouées & Chaînes).
func TestEngineWithRealDatabase(t *testing.T) {
	// 1. Charger l'environnement
	if err := godotenv.Load("../../.env"); err != nil {
		t.Skip("⚠️ .env non trouvé à la racine, test ignoré")
	}

	// 2. Connexion DB
	db.InitDatabase()

	// 3. Récupérer une bouée témoin (ex: la 800)
	var buoy models.Buoy
	if err := db.DB.Where("name = ?", "800").First(&buoy).Error; err != nil {
		t.Fatalf("❌ Impossible de trouver la bouée '800' en BDD : %v", err)
	}
	log.Printf("🏗️ Bouée trouvée : %s, %d tranches flotteur, %d tranches structure", 
		buoy.Name, len(buoy.FlotteurData.Elements), len(buoy.StructureData.Elements))

	// 4. Récupérer une chaîne témoin (ex: 3D DN14)
	var chain models.Chain
	if err := db.DB.Where("type = ? AND dn = ?", "3D", 14).First(&chain).Error; err != nil {
		t.Fatalf("❌ Impossible de trouver la chaîne '3D DN14' en BDD : %v", err)
	}

	// 5. Paramètres de Simulation
	params := calc.SimulationParams{
		Buoy:          buoy,
		Chain:         chain,
		NumBallast:    2,
		LestDensity:   7.0,   // Fonte
		AnchorDensity: 2.4,   // Béton
	}

	// Conditions du site (Modérées pour une petite bouée 800L)
	site := calc.SiteConditions{
		WaterDepth:      10.0,
		WindVelocity:    10.0, // m/s (36 km/h)
		CurrentVelocity: 0.5,  // m/s
		WaveHeight:      1.0,
		WavePeriod:      4.0,
		Marnage:         1.0,
	}

	// 6. Lancement du Moteur
	result, err := calc.FindEquilibrium(params, site)
	if err != nil {
		t.Fatalf("❌ Le moteur a échoué : %v", err)
	}

	// 7. ASSERTIONS DE VALIDATION (IALA)
	log.Printf("📊 RÉSULTAT TEST [Bouée 800] : Enfoncement=%.3fm, Tension=%.2ft, Franc-Bord=%.2fm", 
		result.Enfoncement, result.TensionMaxMouillage, result.FrancBord)

	if result.Enfoncement <= 0 {
		t.Error("❌ Erreur : La bouée ne s'enfonce pas du tout !")
	}

	if result.TensionMaxMouillage <= 0 {
		t.Error("❌ Erreur : Aucune tension détectée sur la chaîne !")
	}

	if result.FrancBord < 0 {
		t.Error("❌ Erreur : La bouée est coulée (Franc-Bord négatif) !")
	}
}
