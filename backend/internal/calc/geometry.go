/*
 * CALMAR GEOMETRY ENGINE - VOLUMETRIC MODULE
 */
package calc

import (
	"backend/internal/models"
	"math"
)

// CalculateComponentSubmergedVolume calcule le volume immergé d'un composant (Flotteur ou Structure)
func CalculateComponentSubmergedVolume(comp models.ComponentData, immersion float64) float64 {
	if immersion <= 0 {
		return 0
	}
	totalVol := 0.0
	currentH := 0.0

	for _, el := range comp.Elements {
		if immersion <= currentH {
			break // La flottaison est en dessous de cette tranche
		}

		// Hauteur de flottaison DANS cette tranche
		hInElement := immersion - currentH
		if hInElement > el.Hauteur {
			hInElement = el.Hauteur
		}

		// Calcul du volume de cette tranche (tronc de cône)
		b1 := math.Pi * math.Pow(el.DiametreBas/2, 2)
		// Diamètre à la flottaison dans la tranche
		dInter := el.DiametreBas + (hInElement * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
		b2 := math.Pi * math.Pow(dInter/2, 2)

		volTranche := (hInElement / 3.0) * (b1 + math.Sqrt(b1*b2) + b2)
		
		// Retrait de la partie creuse (DI)
		bInt := math.Pi * math.Pow(el.DiametreInt/2, 2)
		volCreux := bInt * hInElement

		actualVol := volTranche - volCreux

		// Application du Ratio de correction (si présent en base)
		if el.Volume > 0 {
			b1Full := math.Pi * math.Pow(el.DiametreBas/2, 2)
			b2Full := math.Pi * math.Pow(el.DiametreHaut/2, 2)
			volGeoTotal := (el.Hauteur / 3.0) * (b1Full + math.Sqrt(b1Full*b2Full) + b2Full)
			volCreuxTotal := bInt * el.Hauteur
			volNetTheo := volGeoTotal - volCreuxTotal
			
			if volNetTheo > 0 {
				ratio := el.Volume / volNetTheo
				actualVol *= ratio
			}
		}

		totalVol += actualVol
		currentH += el.Hauteur
	}
	return totalVol
}

// CalculateComponentSurfaceImmergee calcule la surface de traînée courant (sous l'eau)
func CalculateComponentSurfaceImmergee(comp models.ComponentData, immersion float64) float64 {
	if immersion <= 0 { return 0 }
	totalSurf := 0.0
	currentH := 0.0

	for _, el := range comp.Elements {
		if immersion <= currentH { break }
		hInElement := math.Min(immersion-currentH, el.Hauteur)
		
		// Diamètre moyen de la partie immergée de la tranche
		dInter := el.DiametreBas + (hInElement * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
		dmoy := (el.DiametreBas + dInter) / 2.0
		totalSurf += dmoy * hInElement
		
		currentH += el.Hauteur
	}
	return totalSurf
}

// CalculateComponentSurfaceEmergee calcule la surface de traînée vent (hors de l'eau)
func CalculateComponentSurfaceEmergee(comp models.ComponentData, immersion float64) float64 {
	totalHeight := 0.0
	for _, el := range comp.Elements { totalHeight += el.Hauteur }
	
	if immersion >= totalHeight { return 0 }
	
	totalSurf := 0.0
	currentH := 0.0
	for _, el := range comp.Elements {
		elementTop := currentH + el.Hauteur
		if immersion >= elementTop {
			currentH += el.Hauteur
			continue
		}

		// Partie émergée de la tranche
		hStart := math.Max(0, immersion-currentH)
		hEmergent := el.Hauteur - hStart
		
		dStart := el.DiametreBas + (hStart * (el.DiametreHaut - el.DiametreBas) / el.Hauteur)
		dmoy := (dStart + el.DiametreHaut) / 2.0
		totalSurf += dmoy * hEmergent
		
		currentH += el.Hauteur
	}
	return totalSurf
}

// GetComponentTotalHeight calcul la hauteur totale d'un empilement de tranches
func GetComponentTotalHeight(comp models.ComponentData) float64 {
	h := 0.0
	for _, el := range comp.Elements { h += el.Hauteur }
	return h
}
