package db

import (
	"fmt"
	"log"
	"os"

	"backend/internal/models"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

// Instance globale de la base de données
var DB *gorm.DB

// InitDatabase se connecte à PostgreSQL via les variables d'environnement
// et lance les migrations automatiques (AutoMigrate).
func InitDatabase() {
	// 1. Lecture des variables d'environnement (chargées depuis .env dans main.go)
	host := getEnv("DB_HOST", "localhost")
	user := getEnv("DB_USER", "calmar_user")
	password := getEnv("DB_PASSWORD", "calmar_secret_2024")
	dbname := getEnv("DB_NAME", "calmar")
	port := getEnv("DB_PORT", "5433")

	// 2. Construction du DSN PostgreSQL
	dsn := fmt.Sprintf(
		"host=%s user=%s password=%s dbname=%s port=%s sslmode=disable TimeZone=Europe/Paris",
		host, user, password, dbname, port,
	)

	// 3. Connexion via GORM
	var err error
	DB, err = gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatal("❌ Échec de la connexion à PostgreSQL :", err)
	}

	log.Println("✅ Connexion à PostgreSQL établie !")

	// 4. Auto-Migration : GORM crée / met à jour les tables automatiquement
	log.Println("🔄 Auto-Migration des tables (Buoy, ChainType, Chain)...")
	err = DB.AutoMigrate(&models.Buoy{}, &models.ChainType{}, &models.Chain{}, &models.EquipmentStandard{})
	if err != nil {
		log.Fatal("❌ Échec de l'Auto-Migration :", err)
	}

	log.Println("✅ Tables synchronisées avec succès !")
}

// getEnv lit une variable d'environnement ou retourne la valeur par défaut.
func getEnv(key, defaultValue string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}
