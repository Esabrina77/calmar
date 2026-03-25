package routes

import (
	"backend/internal/api/handlers"
	"github.com/gofiber/fiber/v2"
)

// RegisterBuoyRoutes définit les endpoints liés au catalogue de bouées
func RegisterBuoyRoutes(api fiber.Router) {
	buoys := api.Group("/buoys")

	buoys.Get("/", handlers.GetBuoys)
	buoys.Get("/:id", handlers.GetBuoyDetail)
}
