package main

import (
	"encoding/json"
	"fmt"
	"log"
	"backend/internal/db"
	"backend/internal/models"
	"github.com/joho/godotenv"
)

func main() {
	_ = godotenv.Load("backend/.env")
	db.InitDatabase()

	var buoy models.Buoy
	db.DB.Where("name = ?", "Jet 7000 QI PF5 HV").First(&buoy)

	fmt.Printf("Bouée: %s\n", buoy.Name)
	fmt.Printf("Nombre de Pylônes: %d\n", len(buoy.PyloneData))
	
	data, _ := json.MarshalIndent(buoy.PyloneData, "", "  ")
	fmt.Println("Détails Pylônes:")
	fmt.Println(string(data))
}
