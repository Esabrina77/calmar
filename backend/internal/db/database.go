package db

import (
	"log"

	"backend/internal/models"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

// Instance globale de la base de données
var DB *gorm.DB

// InitDatabase se connecte à PostgreSQL et lance les migrations
func InitDatabase() {
	// 2. Construction de la chaîne de connexion DSN en dur complet pour le dev local Windows
	dsn := "host=localhost user=postgres password=secretpassword dbname=calmar port=5433 sslmode=disable TimeZone=Europe/Paris"

	// 3. Connexion à PostgreSQL via GORM
	var err error
	DB, err = gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatal("❌ Échec de la connexion à la base de données PostgreSQL:", err)
	}

	log.Println("✅ Connexion à PostgreSQL établie avec succès !")

	// 4. Le Pouvoir de l'Auto-Migration
	log.Println("🔄 Lancement de l'Auto-Migration GORM pour la table [Buoys]...")
	err = DB.AutoMigrate(&models.Buoy{})
	if err != nil {
		log.Fatal("❌ Échec de l'Auto-Migration :", err)
	}

	log.Println("✅ Structure de la base de données parfaitement synchronisée avec nos structs Go ! ✨")
}
