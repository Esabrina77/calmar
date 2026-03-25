package routes

import (
	"backend/internal/api/handlers"
	"github.com/gofiber/fiber/v2"
)

// RegisterChainRoutes définit les endpoints pour le catalogue de chaînes
func RegisterChainRoutes(api fiber.Router) {
	chains := api.Group("/chains")

	chains.Get("/", handlers.GetChains)
}
