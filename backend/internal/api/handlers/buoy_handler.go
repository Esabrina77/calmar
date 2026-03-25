package handlers

import (
	"backend/internal/db"
	"backend/internal/models"
	"github.com/gofiber/fiber/v2"
)

// GetBuoys retourne la liste simplifiée des bouées (ID + Nom)
// GET /api/buoys
func GetBuoys(c *fiber.Ctx) error {
	var buoys []models.Buoy
	// On ne sélectionne que ce qui est nécessaire pour alléger la réponse
	if err := db.DB.Select("id, name").Find(&buoys).Error; err != nil {
		return err
	}
	return c.JSON(buoys)
}

// GetBuoyDetail retourne toutes les tranches géométriques d'une bouée
// GET /api/buoys/:id
func GetBuoyDetail(c *fiber.Ctx) error {
	id := c.Params("id")
	var buoy models.Buoy
	if err := db.DB.First(&buoy, id).Error; err != nil {
		return fiber.NewError(404, "Modèle de bouée introuvable")
	}
	return c.JSON(buoy)
}
