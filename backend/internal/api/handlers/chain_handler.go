package handlers

import (
	"backend/internal/db"
	"backend/internal/models"
	"github.com/gofiber/fiber/v2"
)

// GetChains retourne l'intégralité du catalogue des chaînes
// GET /api/chains
func GetChains(c *fiber.Ctx) error {
	var chains []models.Chain
	if err := db.DB.Find(&chains).Error; err != nil {
		return err
	}
	return c.JSON(chains)
}
