package main

import (
	"fmt"
	"log"
	"net/http"
	"os"

	"backend/internal/db"

	"github.com/joho/godotenv"
)

func main() {
	// 1. Chargement des variables depuis le fichier .env à la racine du projet
	// Le binaire est lancé depuis /backend donc ../ remonte à la racine /calmar
	if err := godotenv.Load("../.env"); err != nil {
		log.Println("⚠️  .env non trouvé à la racine, on utilise les variables système")
	}

	// 2. Connexion & Migration PostgreSQL
	db.InitDatabase()

	// 3. Démarrage du serveur HTTP
	port := os.Getenv("API_PORT")
	if port == "" {
		port = "8080"
	}

	fmt.Printf("🚀 Calmar API démarrée sur http://localhost:%s\n", port)

	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, `{"status":"ok","service":"Calmar API"}`)
	})

	http.HandleFunc("/health", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, `{"status":"healthy","db":"connected"}`)
	})

	if err := http.ListenAndServe(":"+port, nil); err != nil {
		log.Fatal("❌ Erreur démarrage serveur :", err)
	}
}