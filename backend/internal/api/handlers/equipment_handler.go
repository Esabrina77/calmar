package handlers

import (
	"backend/internal/db"
	"backend/internal/models"
	"github.com/gofiber/fiber/v2"
)

func GetAllEquipments(c *fiber.Ctx) error {
	var equipments []models.EquipmentStandard
	if err := db.DB.Find(&equipments).Error; err != nil {
		return c.Status(500).JSON(fiber.Map{"error": "Impossible de charger le catalogue"})
	}
	return c.JSON(equipments)
}
