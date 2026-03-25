package routes

import (
	"backend/internal/api/handlers"
	"github.com/gofiber/fiber/v2"
)

// RegisterSimulationRoutes définit les endpoints pour le moteur de calcul IALA
func RegisterSimulationRoutes(api fiber.Router) {
	sim := api.Group("/simulate")

	// Endpoint cœur pour le calcul caténaire
	sim.Post("/", handlers.HandleSimulation)
	
	// Futur : sim.Post("/compare", handlers.HandleComparison)
}
