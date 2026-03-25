package models

import (
	"time"
)

// ElementDimItem représente une tranche de cône tronqué (la géométrie de base).
// Ces données seront stockées en base dans nos champs JSONB.
type ElementDimItem struct {
	Hauteur      float64 `json:"H"`
	DiametreBas  float64 `json:"D0"` // D0 = Diamètre Bas (Low)
	DiametreHaut float64 `json:"D1"` // D1 = Diamètre Haut (High)
	DiametreInt  float64 `json:"DI"`
	Volume       float64 `json:"Volume"`
}

// ComponentData représente un composant avec des tranches (Structure ou Flotteur).
type ComponentData struct {
	Name           string           `json:"Name"`
	Masse          float64          `json:"Masse"`
	OffsetFlotteur float64          `json:"OffsetFlotteur,omitempty"`
	OffsetOrganeau float64          `json:"OffsetOrganeau,omitempty"`
	Elements       []ElementDimItem `json:"Elements,omitempty"`
}

// TopmarkData représente un mât ou un équipement (Pylône, Feu).
type TopmarkData struct {
	Name      string  `json:"Name"`
	Height    float64 `json:"Height"`
	WidthHigh float64 `json:"WidthHigh"`
	WidthLow  float64 `json:"WidthLow"`
	Masse     float64 `json:"Masse"`
}

// Buoy représente le modèle principal stocké dans la base PostgreSQL.
type Buoy struct {
	ID                uint      `gorm:"primaryKey" json:"id"`
	Name              string    `gorm:"type:varchar(255);uniqueIndex;not null" json:"name"`
	ChaineMin         int       `json:"chaine_min"`
	ChaineMax         int       `json:"chaine_max"`
	MasseLestUnitaire float64   `json:"masse_lest_unitaire"`
	NombreLestMin     int       `json:"nombre_lest_min"`
	NombreLestMax     int       `json:"nombre_lest_max"`

	// L'utilisation de `serializer:json` permet à GORM de convertir automatiquement
	// ces structs internes en colonnes JSONB sous PostgreSQL.
	// C'est toute la puissance et la souplesse évoquée !
	StructureData  ComponentData `gorm:"type:jsonb;serializer:json" json:"structure"`
	FlotteurData   ComponentData `gorm:"type:jsonb;serializer:json" json:"flotteur"`
	PyloneData     TopmarkData   `gorm:"type:jsonb;serializer:json" json:"pylone"`
	EquipementData TopmarkData   `gorm:"type:jsonb;serializer:json" json:"equipement"`

	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}
