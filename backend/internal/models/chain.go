package models

import "time"

// ChainType représente une catégorie de chaîne (3D, 3.5D, Stud, etc.)
type ChainType struct {
	ID        uint      `gorm:"primaryKey" json:"id"`
	Name      string    `gorm:"type:varchar(50);not null;unique" json:"name"`
	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}

// Chain représente une variante spécifique d'une chaîne (Type + DN).
type Chain struct {
	ID             uint      `gorm:"primaryKey" json:"id"`
	ChainTypeID    uint      `json:"chain_type_id"`
	ChainType      ChainType `gorm:"foreignKey:ChainTypeID" json:"chain_type"`
	Type           string    `gorm:"type:varchar(50);not null" json:"type"` // Gardé pour compatibilité ou affichage direct
	DN             float64   `gorm:"not null" json:"dn"`                  // Diamètre Nominal (mm)
	MasseLineique  float64   `json:"masse_lineique"`                      // kg/m dans l'air
	ChargeEpreuveQ1 float64   `json:"charge_epreuve_q1"`                   // t (Qualité 1)
	ChargeEpreuveQ2 float64   `json:"charge_epreuve_q2"`                   // t (Qualité 2)
	ChargeEpreuveQ3 float64   `json:"charge_epreuve_q3"`                   // t (Qualité 3)

	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}
