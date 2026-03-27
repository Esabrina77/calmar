package main

import (
	"backend/internal/db"
	"backend/internal/models"
	"encoding/xml"
	"fmt"
	"io"
	"log"
	"os"
	"strings"
	"strconv"

	"github.com/joho/godotenv"
)

type EquipementStandardItem struct {
	Categorie     string `xml:"Categorie,attr"`
	Name          string `xml:"Name,attr"`
	MasseUnitaire string `xml:"MasseUnitaire,attr"`
}

type EquipementsXML struct {
	Items []EquipementStandardItem `xml:"EquipementSupplementaire>EquipementSupplementaireElementItem"`
}

func parseDoubleLocal(s string) float64 {
	s = strings.ReplaceAll(s, ",", ".")
	val, _ := strconv.ParseFloat(s, 64)
	return val
}

func main() {
	_ = godotenv.Load("../.env")
	db.InitDatabase()

	fmt.Println("🧹 Nettoyage du catalogue d'équipements...")
	db.DB.Exec("TRUNCATE TABLE equipment_standards CASCADE")

	// Open XML
	xmlPath := "c:/Users/kapor/Desktop/calmar/old-calmar/WorkingDirectory/Equipements.xmlMB"
	xmlFile, err := os.Open(xmlPath)
	if err != nil {
		log.Fatal("❌ Impossible d'ouvrir le fichier XML :", err)
	}
	defer xmlFile.Close()

	byteValue, _ := io.ReadAll(xmlFile)
	var equipements EquipementsXML
	xml.Unmarshal(byteValue, &equipements)

	fmt.Printf("📂 %d équipements trouvés dans le fichier XML.\n", len(equipements.Items))

	for _, item := range equipements.Items {
		eq := models.EquipmentStandard{
			Categorie:     item.Categorie,
			Name:          item.Name,
			MasseUnitaire: parseDoubleLocal(item.MasseUnitaire),
		}

		if err := db.DB.Create(&eq).Error; err != nil {
			log.Printf("⚠️ Erreur création équipement %s : %v", item.Name, err)
		}
	}

	fmt.Println("🎉 Importation du catalogue terminée !")
}
