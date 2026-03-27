package routes

import (
	"backend/internal/api/handlers"
	"github.com/gofiber/fiber/v2"
)

func RegisterEquipmentRoutes(router fiber.Router) {
	group := router.Group("/equipments")
	group.Get("", handlers.GetAllEquipments)
}
