package handlers

import (
	"backend/internal/calc"
	"backend/internal/db"
	"backend/internal/models"
	"fmt"
	"github.com/gofiber/fiber/v2"
)

// SimulationRequest définit le format attendu du Frontend pour lancer un calcul
type SimulationRequest struct {
	BuoyID        uint                `json:"buoy_id"`
	ChainID       uint                `json:"chain_id"`
	NumBallast    int                 `json:"num_ballast"`
	EquipmentMass float64             `json:"equipment_mass"`
	ChainQuality  string              `json:"chain_quality"` // Q1, Q2, Q3
	AnchorDensity float64             `json:"anchor_density"`
	Conditions    calc.SiteConditions `json:"conditions"`
}

// HandleSimulation orchestre l'appel au moteur physique IALA
// POST /api/simulate
func HandleSimulation(c *fiber.Ctx) error {
	req := new(SimulationRequest)
	if err := c.BodyParser(req); err != nil {
		return fiber.NewError(400, "Données de simulation invalides (Body JSON)")
	}

	// 1. Fetch Buoy Data
	var buoy models.Buoy
	if err := db.DB.First(&buoy, req.BuoyID).Error; err != nil {
		return fiber.NewError(404, "Modèle de bouée non répertorié")
	}

	// 2. Fetch Chain Data
	var chain models.Chain
	if err := db.DB.First(&chain, req.ChainID).Error; err != nil {
		return fiber.NewError(404, "Type de chaîne non trouvé")
	}

	// Mapping de la Charge d'Épreuve selon la qualité Q1, Q2, Q3
	chargeEpreuve := chain.ChargeEpreuveQ1
	switch req.ChainQuality {
	case "Q2":
		chargeEpreuve = chain.ChargeEpreuveQ2
	case "Q3":
		chargeEpreuve = chain.ChargeEpreuveQ3
	}

	// 3. Setup Physics Engine Params
	simParams := calc.SimulationParams{
		Buoy:                buoy,
		Chain:               chain,
		NumBallast:          req.NumBallast,
		ExtraEquipmentMass:  req.EquipmentMass,
		LestDensity:         calc.BallastDensity,
		AnchorDensity:       req.AnchorDensity,
		ChainQualityCharge:  chargeEpreuve, // Nouvelle valeur injectée
	}

	// 4. Critical Calculation (IALA compliant)
	result, err := calc.FindEquilibrium(simParams, req.Conditions)
	if err != nil {
		return fiber.NewError(500, "Échec du moteur physique : "+err.Error())
	}

	// DEBUG LOG
	fmt.Printf("📊 RÉSULTAT BRUT : %+v\n", result)

	return c.JSON(result)
}
