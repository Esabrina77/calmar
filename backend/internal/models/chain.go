package models

import "time"

// Chain représente une chaîne de mouillage standard (IALA).
type Chain struct {
	ID             uint      `gorm:"primaryKey" json:"id"`
	Type           string    `gorm:"type:varchar(50);not null" json:"type"` // Ex: "3D", "Stud", etc.
	DN             float64   `gorm:"not null" json:"dn"`                  // Diamètre Nominal (mm)
	MasseLineique  float64   `json:"masse_lineique"`                      // kg/m dans l'air
	ChargeEpreuveQ1 float64   `json:"charge_epreuve_q1"`                   // t (Qualité 1)
	ChargeEpreuveQ2 float64   `json:"charge_epreuve_q2"`                   // t (Qualité 2)
	ChargeEpreuveQ3 float64   `json:"charge_epreuve_q3"`                   // t (Qualité 3)

	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}
