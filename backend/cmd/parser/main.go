package main

import (
	"encoding/xml"
	"fmt"
	"log"
	"os"
	"path/filepath"
	"strings"

	"backend/internal/models"
	"backend/internal/db"
)

// --- Structures XML intermédiaires ---
// Celles-ci servent uniquement à lire les balises XML exactes de vos fichiers (.xmlMB)
type XMLCalmar struct {
	XMLName xml.Name `xml:"Calmar"`
	Buoy    XMLBuoy  `xml:"Buoy"`
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
	Pylone            XMLTopmark   `xml:"Pylone"`
	Equipement        XMLTopmark   `xml:"Equipement"`
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

func main() {
	// On initialise la connexion PostgreSQL et la table
	db.InitDatabase()

	fmt.Println("🚀 Démarrage du Parser de Bouées Calmar...")
	
	// Chemin vers vos fichiers de tests
	dirPath := filepath.Join("tests", "fixtures")
	
	files, err := os.ReadDir(dirPath)
	if err != nil {
		log.Fatalf("❌ Impossible de lire le dossier %s : %v", dirPath, err)
	}

	successCount := 0

	for _, file := range files {
		// On ignore les dossiers ou les fichiers cryptés pour le moment
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

		// --- MAPPING ---
		buoyModel := models.Buoy{
			Name:              doc.Buoy.Name,
			ChaineMin:         doc.Buoy.ChaineMin,
			ChaineMax:         doc.Buoy.ChaineMax,
			MasseLestUnitaire: doc.Buoy.MasseLestUnitaire,
			NombreLestMin:     doc.Buoy.NombreLestMin,
			NombreLestMax:     doc.Buoy.NombreLestMax,
			StructureData:     mapComponent(doc.Buoy.Structure),
			FlotteurData:      mapComponent(doc.Buoy.Flotteur),
			PyloneData:        mapTopmark(doc.Buoy.Pylone),
			EquipementData:    mapTopmark(doc.Buoy.Equipement),
		}

		// --- SAUVEGARDE EN BDD ---
		// On sauvegarde magiquement en un appel dans la DB Postgres !
		result := db.DB.Create(&buoyModel)
		if result.Error != nil {
			log.Printf("❌ Impossible de sauvegarder la bouée [%s] : %v", buoyModel.Name, result.Error)
			continue
		}
		
		fmt.Printf("✅ DB Succès : L'objet Go de la bouée [%s] a été sauvegardé en Base de Données ! 🎉\n", buoyModel.Name)
		successCount++
	}

	fmt.Printf("🎉 Fin du script ! %d modèles XML ont été parsés avec succès et sont prêts à rejoindre la BD.\n", successCount)
}

// mapComponent convertit un composant XML en ComponentData de notre Modèle GORM
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
			DiametreHaut: el.D0,
			DiametreBas:  el.D1,
			DiametreInt:  el.DI,
			Volume:       el.Volume,
		})
	}
	return comp
}

// mapTopmark convertit un XMLTopmark en TopmarkData de notre Modèle GORM
func mapTopmark(xmlTop XMLTopmark) models.TopmarkData {
	return models.TopmarkData{
		Name:      xmlTop.Name,
		Height:    xmlTop.Height,
		WidthHigh: xmlTop.WidthHigh,
		WidthLow:  xmlTop.WidthLow,
		Masse:     xmlTop.Masse,
	}
}
