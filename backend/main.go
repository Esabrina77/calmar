package main

import (
	"log"

	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/cors"
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

// Déclaration de la variable globale DB (pour tests)
var DB *gorm.DB

func main() {
	// 1. Initialisation de Fiber
	app := fiber.New(fiber.Config{
		AppName: "Calmar API v1.0",
	})

	// 2. Configuration CORS (pour laisser Next.js appeler l'API)
	app.Use(cors.New(cors.Config{
		AllowOrigins: "http://localhost:3000",
		AllowHeaders: "Origin, Content-Type, Accept",
	}))

	// 🛠️ 3. Placeholder Connexion DB (Activable plus tard)
	// connectDatabase()

	// 4. Définition des Routes
	setupRoutes(app)

	// 5. Lancement du serveur sur le port 8000
	log.Fatal(app.Listen(":8000"))
}

func setupRoutes(app *fiber.App) {
	// Route d'accueil
	app.Get("/", func(c *fiber.Ctx) error {
		return c.JSON(fiber.Map{
			"status":  "online",
			"message": "Bienvenue sur l'API Calmar (Golang 🐹)",
			"version": "1.0.0",
		})
	})

	// Route de Santé (HealthCheck)
	app.Get("/health", func(c *fiber.Ctx) error {
		return c.JSON(fiber.Map{
			"status": "ok",
		})
	})
}

// Fonction de placeholder pour GORM
func connectDatabase() {
	// DSN (Data Source Name) - À configurer via Var d'environnement
	dsn := "host=localhost user=postgres password=secret dbname=calmar port=5432 sslmode=disable"
	var err error
	DB, err = gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Println("⚠️ Impossible de se connecter à la DB (Placeholder). Configurer le DSN.")
	} else {
		log.Println("✅ Base de données PostgreSQL connectée avec GORM.")
	}
}
