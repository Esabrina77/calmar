package api

import (
	"backend/internal/api/middleware"
	"backend/internal/api/routes"
	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/cors"
	"github.com/gofiber/fiber/v2/middleware/logger"
)

// SetupApp configure l'instance du serveur Fiber avec une architecture modulaire.
func SetupApp() *fiber.App {
	app := fiber.New(fiber.Config{
		AppName:      "Calmar Physics Engine API",
		ErrorHandler: middleware.ErrorHandler, // Gestion centralisée des erreurs
	})

	// 1. GLOBAL MIDDLEWARES
	app.Use(logger.New())
	app.Use(cors.New())

	// 2. API GROUP
	api := app.Group("/api")

	// 3. ENREGISTREMENT DES MODULES ROUTE
	// Chaque domaine métier est isolé
	routes.RegisterBuoyRoutes(api)      // Gestion des modèles de bouées
	routes.RegisterChainRoutes(api)     // Gestion du catalogue de chaînes
	routes.RegisterSimulationRoutes(api) // Moteur de calcul caténaire

	// Health Check
	app.Get("/health", func(c *fiber.Ctx) error {
		return c.SendString("Calmar API is running 🚀")
	})

	return app
}
