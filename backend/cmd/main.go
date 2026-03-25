package main

import (
	"fmt"
	"log"
	"os"

	"backend/internal/api"
	"backend/internal/db"

	"github.com/joho/godotenv"
)

func main() {
	// 1. Chargement des variables .env
	if err := godotenv.Load("../.env"); err != nil {
		log.Println("⚠️  .env non trouvé à la racine, on utilise les variables système")
	}

	// 2. Connexion & Migration PostgreSQL
	db.InitDatabase()

	// 3. Démarrage du serveur Fiber
	app := api.SetupApp()
	
	port := os.Getenv("API_PORT")
	if port == "" {
		port = "4000"
	}

	fmt.Printf("🚀 Calmar API démarrée sur http://localhost:%s\n", port)
	if err := app.Listen(":" + port); err != nil {
		log.Fatal("❌ Erreur démarrage serveur Fiber :", err)
	}
}