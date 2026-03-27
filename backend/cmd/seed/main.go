package main

import (
	"backend/internal/db"
	"backend/internal/models"
	"encoding/xml"
	"fmt"
	"io"
	"log"
	"os"
	"strconv"
	"strings"

	"github.com/joho/godotenv"
)

type ChainElementItem struct {
	Type     string `xml:"Type,attr"`
	DN       string `xml:"DN,attr"`
	MasseLin string `xml:"MasseLin,attr"`
	Q1_CE    string `xml:"Q1_CE,attr"`
	Q2_CE    string `xml:"Q2_CE,attr"`
	Q3_CE    string `xml:"Q3_CE,attr"`
}

type CalmarXML struct {
	Items []ChainElementItem `xml:"Chain>ChainElementItem"`
}

func parseDouble(s string) float64 {
	s = strings.ReplaceAll(s, ",", ".")
	val, _ := strconv.ParseFloat(s, 64)
	return val
}

func main() {
	// 1. Load env
	_ = godotenv.Load("../.env")

	// 2. Init DB
	db.InitDatabase()

	fmt.Println("🧹 Nettoyage des tables de chaînes...")
	db.DB.Exec("TRUNCATE TABLE chains CASCADE")
	db.DB.Exec("TRUNCATE TABLE chain_types CASCADE")

	// 3. Open XML
	xmlPath := "c:/Users/kapor/Desktop/calmar/old-calmar/WorkingDirectory/Chains.xmlMB"
	xmlFile, err := os.Open(xmlPath)
	if err != nil {
		log.Fatal("❌ Impossible d'ouvrir le fichier XML :", err)
	}
	defer xmlFile.Close()

	byteValue, _ := io.ReadAll(xmlFile)
	var calmar CalmarXML
	xml.Unmarshal(byteValue, &calmar)

	fmt.Printf("📂 %d entrées trouvées dans le fichier XML.\n", len(calmar.Items))

	fmt.Println("🌱 Création des types et des variantes...")
	typeMap := make(map[string]uint)

	for _, item := range calmar.Items {
		// Create type if not exists
		if _, exists := typeMap[item.Type]; !exists {
			ct := models.ChainType{Name: item.Type}
			db.DB.Create(&ct)
			typeMap[item.Type] = ct.ID
		}

		// Create chain
		chain := models.Chain{
			ChainTypeID:     typeMap[item.Type],
			Type:            item.Type,
			DN:              parseDouble(item.DN),
			MasseLineique:   parseDouble(item.MasseLin),
			ChargeEpreuveQ1: parseDouble(item.Q1_CE),
			ChargeEpreuveQ2: parseDouble(item.Q2_CE),
			ChargeEpreuveQ3: parseDouble(item.Q3_CE),
		}

		if err := db.DB.Create(&chain).Error; err != nil {
			log.Printf("⚠️ Erreur création chaine %s DN %s : %v", item.Type, item.DN, err)
		}
	}

	fmt.Println("🎉 Importation massive terminée avec succès !")
}
