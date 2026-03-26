package main

import (
	"encoding/xml"
	"fmt"
	"log"
	"os"
	"path/filepath"
	"strconv"
	"strings"

	"backend/internal/db"
	"backend/internal/models"

	"github.com/joho/godotenv"
)

// --- Structures XML intermédiaires ---

type XMLCalmar struct {
	XMLName xml.Name `xml:"Calmar"`
	Buoy    XMLBuoy  `xml:"Buoy"`
	Chain   XMLChain `xml:"Chain"`
}

type XMLBuoy struct {
	Name              string       `xml:"Name,attr"`
	ChaineMin         int          `xml:"ChaineMin,attr"`
	ChaineMax         int          `xml:"ChaineMax,attr"`
	MasseLestUnitaire float64      `xml:"MasseLestUnitaire,attr"`
	NombreLestMin     int          `xml:"NombreLestMin,attr"`
	NombreLestMax     int          `xml:"NombreLestMax,attr"`
	Structure         XMLComponent `xml:"Structure"`
	Flotteur          XMLComponent `xml:"Flotteur"`
	Pylone            []XMLTopmark `xml:"Pylone"`
	Equipement        []XMLTopmark `xml:"Equipement"`
}

type XMLComponent struct {
	Name           string       `xml:"Name,attr"`
	Masse          float64      `xml:"Masse,attr"`
	OffsetFlotteur float64      `xml:"OffsetFlotteur,attr"`
	OffsetOrganeau float64      `xml:"OffsetOrganeau,attr"`
	Elements       []XMLElement `xml:"ElementDimItem"`
}

type XMLElement struct {
	H      float64 `xml:"H,attr"`
	D0     float64 `xml:"D0,attr"`
	D1     float64 `xml:"D1,attr"`
	DI     float64 `xml:"DI,attr"`
	Volume float64 `xml:"Volume,attr"`
}

type XMLTopmark struct {
	Name      string  `xml:"Name,attr"`
	Height    float64 `xml:"Height,attr"`
	WidthHigh float64 `xml:"WidthHigh,attr"`
	WidthLow  float64 `xml:"WidthLow,attr"`
	Masse     float64 `xml:"Masse,attr"`
}

type XMLChain struct {
	Items []XMLChainItem `xml:"ChainElementItem"`
}

type XMLChainItem struct {
	Type     string `xml:"Type,attr"`
	DN       string `xml:"DN,attr"`       // String pour gérer les virgules (ex: 12,5)
	MasseLin string `xml:"MasseLin,attr"` // String pour gérer les virgules
	Q1_CE    string `xml:"Q1_CE,attr"`
	Q2_CE    string `xml:"Q2_CE,attr"`
	Q3_CE    string `xml:"Q3_CE,attr"`
}

func main() {
	// 1. Chargement des variables .env
	_ = godotenv.Load("../.env")

	// 2. Initialisation DB
	db.InitDatabase()
	db.DB.Exec("TRUNCATE TABLE buoys CASCADE")

	fmt.Println("🚀 Démarrage du Parser Calmar (Bouées & Chaînes)...")

	dirPath := filepath.Join("tests", "fixtures")
	files, err := os.ReadDir(dirPath)
	if err != nil {
		log.Fatalf("❌ Impossible de lire le dossier %s : %v", dirPath, err)
	}

	for _, file := range files {
		if file.IsDir() || !strings.HasSuffix(file.Name(), ".xmlMB") {
			continue
		}

		filePath := filepath.Join(dirPath, file.Name())
		data, err := os.ReadFile(filePath)
		if err != nil {
			log.Printf("⚠️ Erreur de lecture pour %s : %v", file.Name(), err)
			continue
		}

		var doc XMLCalmar
		err = xml.Unmarshal(data, &doc)
		if err != nil {
			log.Printf("⚠️ Erreur de parsing XML pour %s : %v", file.Name(), err)
			continue
		}

		fmt.Printf("🔍 Traitement de %s (Buoy: '%s', ChainItems: %d)\n", file.Name(), doc.Buoy.Name, len(doc.Chain.Items))

		// --- CAS 1 : C'est un de fichier de CHAÎNES ---
		if len(doc.Chain.Items) > 0 {
			fmt.Printf("⛓️  Parsing du catalogue de chaînes [%s]...\n", file.Name())
			for _, ci := range doc.Chain.Items {
				chain := models.Chain{
					Type:            ci.Type,
					DN:              parseFloat(ci.DN),
					MasseLineique:   parseFloat(ci.MasseLin),
					ChargeEpreuveQ1: parseFloat(ci.Q1_CE),
					ChargeEpreuveQ2: parseFloat(ci.Q2_CE),
					ChargeEpreuveQ3: parseFloat(ci.Q3_CE),
				}
				db.DB.Where(models.Chain{Type: chain.Type, DN: chain.DN}).FirstOrCreate(&chain)
			}
			fmt.Printf("✅ %d chaînes traitées.\n", len(doc.Chain.Items))
			continue
		}

		// --- CAS 2 : C'est un fichier de BOUÉE ---
		if doc.Buoy.Name != "" {
			buoyModel := models.Buoy{
				Name:              doc.Buoy.Name,
				ChaineMin:         doc.Buoy.ChaineMin,
				ChaineMax:         doc.Buoy.ChaineMax,
				MasseLestUnitaire: doc.Buoy.MasseLestUnitaire,
				NombreLestMin:     doc.Buoy.NombreLestMin,
				NombreLestMax:     doc.Buoy.NombreLestMax,
				StructureData:     mapComponent(doc.Buoy.Structure),
				FlotteurData:      mapComponent(doc.Buoy.Flotteur),
				PyloneData:        mapTopmarks(doc.Buoy.Pylone),
				EquipementData:    mapTopmarks(doc.Buoy.Equipement),
			}
			db.DB.Where(models.Buoy{Name: buoyModel.Name}).FirstOrCreate(&buoyModel)
			fmt.Printf("✅ Bouée [%s] synchronisée en BDD.\n", buoyModel.Name)
		}
	}

	fmt.Println("🎉 Fin du parsing !")
}

func mapComponent(xmlComp XMLComponent) models.ComponentData {
	comp := models.ComponentData{
		Name:           xmlComp.Name,
		Masse:          xmlComp.Masse,
		OffsetFlotteur: xmlComp.OffsetFlotteur,
		OffsetOrganeau: xmlComp.OffsetOrganeau,
	}
	for _, el := range xmlComp.Elements {
		comp.Elements = append(comp.Elements, models.ElementDimItem{
			Hauteur:      el.H,
			DiametreBas:  el.D0, // D0 = Bas
			DiametreHaut: el.D1, // D1 = Haut
			DiametreInt:  el.DI,
			Volume:       el.Volume,
		})
	}
	return comp
}

func mapTopmarks(xmlTops []XMLTopmark) []models.TopmarkData {
	var tops []models.TopmarkData
	for _, xt := range xmlTops {
		tops = append(tops, models.TopmarkData{
			Name:      xt.Name,
			Height:    xt.Height,
			WidthHigh: xt.WidthHigh,
			WidthLow:  xt.WidthLow,
			Masse:     xt.Masse,
		})
	}
	return tops
}

func parseFloat(s string) float64 {
	s = strings.ReplaceAll(s, ",", ".")
	val, _ := strconv.ParseFloat(s, 64)
	return val
}
