package handlers

import (
	"backend/internal/calc"
	"backend/internal/db"
	"backend/internal/models"
	"github.com/gofiber/fiber/v2"
)

// SimulationRequest définit le format attendu du Frontend pour lancer un calcul
type SimulationRequest struct {
	BuoyID             uint                `json:"buoy_id"`
	ChainID            uint                `json:"chain_id"`
	NumBallast         int                 `json:"num_ballast"`
	ExtraEquipmentMass float64             `json:"extra_mass"`
	SiteConditions     calc.SiteConditions `json:"conditions"`
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

	// 3. Setup Physics Engine Params
	simParams := calc.SimulationParams{
		Buoy:               buoy,
		Chain:              chain,
		NumBallast:         req.NumBallast,
		ExtraEquipmentMass: req.ExtraEquipmentMass,
		LestDensity:        calc.BallastDensity,
		AnchorDensity:      2400.0, // Default concrete
	}

	// 4. Critical Calculation (IALA compliant)
	result, err := calc.FindEquilibrium(simParams, req.SiteConditions)
	if err != nil {
		return fiber.NewError(500, "Échec du moteur physique : "+err.Error())
	}

	return c.JSON(result)
}
