package calc

import "math"

// --- STRUCTURES GÉOMÉTRIQUES ---

// TroncConeElement correspond à un segment de bouée (Flotteur ou Structure).
// Équivalent à la classe VB.NET CDimensionElementTroncCone.vb
type TroncConeElement struct {
	DiameterLow    float64 // _Diameter_L (Mètres)
	DiameterHigh   float64 // _Diameter_H (Mètres)
	DiameterInter  float64 // _Diameter_I (Mètres) - Partie Creuse (Tube)
	HauteurElement float64 // Hauteur totale du segment (Mètres)
	VolumeReel     float64 // _Volume (Spécifié/Ajusté si disponible)
}

// --- FONCTIONS GÉOMÉTRIQUES PURES ---

// GetSurfaceDiameterLow calcule la surface du diamètre bas (B1).
func (e *TroncConeElement) GetSurfaceDiameterLow() float64 {
	return math.Pi * math.Pow(e.DiameterLow/2, 2)
}

// GetSurfaceDiameterHigh calcule la surface du diamètre haut (B2).
func (e *TroncConeElement) GetSurfaceDiameterHigh() float64 {
	return math.Pi * math.Pow(e.DiameterHigh/2, 2)
}

// GetSurfaceDiameterInter calcule la surface du diamètre creux (Inter).
func (e *TroncConeElement) GetSurfaceDiameterInter() float64 {
	return math.Pi * math.Pow(e.DiameterInter/2, 2)
}

// CalculVolumeTroncDeCone calcule le volume géométrique d'un tronc de cône.
// Formule : H / 3 * (B1 + Sqrt(B1 * B2) + B2)
func CalculVolumeTroncDeCone(h float64, b1 float64, b2 float64) float64 {
	if h <= 0 {
		return 0
	}
	return (h / 3.0) * (b1 + math.Sqrt(b1*b2) + b2)
}

// GetVolumeCalcule calcule le Volume Théorique (Plein - Creux).
func (e *TroncConeElement) GetVolumeCalcule() float64 {
	volPlein := CalculVolumeTroncDeCone(e.HauteurElement, e.GetSurfaceDiameterLow(), e.GetSurfaceDiameterHigh())
	volVide := CalculVolumeTroncDeCone(e.HauteurElement, e.GetSurfaceDiameterInter(), e.GetSurfaceDiameterInter())
	return volPlein - volVide
}

// GetVolume retourne le volume effectif (Réel si renseigné, sinon Calculé).
func (e *TroncConeElement) GetVolume() float64 {
	if e.VolumeReel > 0 {
		return e.VolumeReel
	}
	return e.GetVolumeCalcule()
}

// GetRatioVolume calcule le coefficient correcteur de volume pour les tranches d'immersion.
// Équivalent VB.NET : Volume / VolumeCalcule
func (e *TroncConeElement) GetRatioVolume() float64 {
	volCalc := e.GetVolumeCalcule()
	if volCalc <= 0 {
		return 1.0
	}
	// On applique le volume effectif sur le volume théorique
	return e.GetVolume() / volCalc
}

// VolumeByHauteur calcule le volume immergé d'un segment à une hauteur H donnée.
// Équivalent à VolumeByHauteur(H) de CDimensionElementTroncCone.vb
func (e *TroncConeElement) VolumeByHauteur(h float64) float64 {
	// 1. Protection contre les hauteurs invalides ou supérieures
	if h <= 0 {
		return 0
	}
	if h >= e.HauteurElement {
		return e.GetVolume() // Retourner le volume maximum du segment
	}

	// 2. Calcul du Diamètre Intermédiaire (L_Inter) à la hauteur H
	// Formule : D_bas + (H * (D_haut - D_bas) / HauteurTotale)
	L_Inter := e.DiameterLow + (h * (e.DiameterHigh - e.DiameterLow) / e.HauteurElement)

	// 3. Calcul de la Surface intermédiaire au point de flottaison
	surfaceInter := math.Pi * math.Pow(L_Inter/2, 2)

	// 4. Volume immergé de la partie "Pleine"
	volPleinImmerge := CalculVolumeTroncDeCone(h, e.GetSurfaceDiameterLow(), surfaceInter)

	// 5. Volume immergé de la partie "Creuse" (Cylindre classique)
	surfaceCreuse := e.GetSurfaceDiameterInter()
	volCreuxImmerge := CalculVolumeTroncDeCone(h, surfaceCreuse, surfaceCreuse)

	// 6. Application du Ratio de correction d'ajustement VB.NET
	volImmergeStandard := volPleinImmerge - volCreuxImmerge
	return volImmergeStandard * e.GetRatioVolume()
}
