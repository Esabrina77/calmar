/*
 * CALMAR PHYSICS ENGINE - IALA COMPLIANCE MODULE
 * --------------------------------------------
 * Ce fichier contient les constantes physiques et les formules IALA
 * transposées du projet legacy Calmar (VB.NET).
 */
package calc

import "math"

// --- Constantes Alignées sur VB.NET ---
const (
	WaterDensity   = 1025.0 // kg/m3 (eau salée)
	AirDensity     = 1.225  // kg/m3 (air sec à 15°C)
	MetalDensity   = 7850.0 // kg/m3 (Acier standard)
	BallastDensity = 7320.0 // kg/m3 (Fonte ductile)
	Gravity        = 9.81   // m/s²
)

// CalculateBuoyancy calcule la poussée d'Archimède (N)
func CalculateBuoyancy(volumeImmerge float64) float64 {
	return volumeImmerge * WaterDensity * Gravity
}

// CalculateWindDrag calcule les Traînées de Vent (kgf)
func CalculateWindDrag(vitesseVent float64, surfaceEmergee float64) float64 {
	const cdWind = 1.2
	return (0.5 * (vitesseVent * vitesseVent) * AirDensity * (cdWind * surfaceEmergee)) / Gravity
}

// CalculateChainDrag calcule l'effort du courant sur la CHAINE (kgf)
func CalculateChainDrag(vitesseCourant float64, surfaceChaine float64) float64 {
	const cdChain = 1.2
	return (0.5 * (vitesseCourant * vitesseCourant) * WaterDensity * (cdChain * surfaceChaine)) / Gravity
}

// CalculateBuoyCurrentDrag calcule l'effort du courant sur la BOUEE (kgf)
func CalculateBuoyCurrentDrag(vitesseCourantSurface float64, surfaceImmergee float64) float64 {
	const cdCurrent = 1.2
	return (0.5 * (vitesseCourantSurface * vitesseCourantSurface) * WaterDensity * (cdCurrent * surfaceImmergee)) / Gravity
}

// CalculateCatenaryLength calcule S^2 (Equation de la ligne de mouillage)
func CalculateCatenaryLength(hauteurCatenaire float64, effortHorizontalKg float64, poidsLineiqueImmerge float64) float64 {
	if poidsLineiqueImmerge <= 0 {
		return math.Pow(hauteurCatenaire, 2)
	}
	ratio := effortHorizontalKg / poidsLineiqueImmerge
	return (hauteurCatenaire * hauteurCatenaire) + (2.0 * ratio * hauteurCatenaire)
}

func CalculateLineicWeight(massLineiqueAir float64) float64 {
	return massLineiqueAir * (1.0 - (WaterDensity / MetalDensity))
}

func CalculateSubmergedBallastWeight(massBallastAir float64) float64 {
	return massBallastAir * (1.0 - (WaterDensity / BallastDensity))
}

// CalculateSubmergedBallastWeightDynamic avec densité d'eau variable
func CalculateSubmergedBallastWeightDynamic(massBallastAir float64, waterDensity float64) float64 {
	return massBallastAir * (1.0 - (waterDensity / BallastDensity))
}

func CalculateTotalHorizontalEffort(windDrag float64, currentDragBuoy float64, currentDragChain float64) float64 {
	return windDrag + currentDragBuoy + currentDragChain
}

func ACosh(x float64) float64 {
	if x < 1 {
		return 0
	}
	return math.Log(x + math.Sqrt(x*x-1))
}

// CalculateSwingRadius (Rayon d'évitage)
// Correction : effortHorizontalKg est déjà en KG, on ne multiplie plus par 1000 !
func CalculateSwingRadius(effortHorizontalKg float64, poidsLineique float64, hauteurCatenaire float64) float64 {
	if poidsLineique <= 0 {
		return 0
	}
	forceRatio := effortHorizontalKg / poidsLineique
	if forceRatio <= 0 {
		return 0
	}
	return forceRatio * ACosh((hauteurCatenaire/forceRatio)+1.0)
}

func CalculateMinAnchorMass(effortHorizontalKg float64, densiteCM float64) float64 {
	const seaBedFrictionAngle = 45.0
	const safetyFactor = 1.5
	if densiteCM <= WaterDensity/1000.0 {
		return 0
	}
	angleRad := seaBedFrictionAngle * math.Pi / 180.0
	return safetyFactor * (effortHorizontalKg / 1000.0) * (densiteCM / (densiteCM - WaterDensity/1000.0)) / math.Tan(angleRad) * 1000.0
}

func CalculateSubmergedAnchorWeight(massCorpsMort float64, densiteCM float64) float64 {
	if densiteCM <= 0 {
		return 0
	}
	return massCorpsMort / (densiteCM * 1000.0) * (densiteCM*1000.0 - WaterDensity)
}

func CalculateChainSafetyCoefficient(chargeEpreuve float64, hauteurCatenaire float64, poidsLineique float64, effortHorizontalKg float64) float64 {
	tensionVerticale := (hauteurCatenaire * poidsLineique)
	denom := (tensionVerticale + effortHorizontalKg)
	if denom <= 0 {
		return 0
	}
	return chargeEpreuve / denom
}

func CalculateTangencyAngle(poidsCatenaireImmerge float64, tensionChaineTonnes float64) float64 {
	if tensionChaineTonnes <= 0 {
		return 0
	}
	cosAngle := (poidsCatenaireImmerge / 1000.0) / tensionChaineTonnes
	cosAngle = math.Max(-1.0, math.Min(1.0, cosAngle))
	return math.Acos(cosAngle) * 180.0 / math.Pi
}

func CalculateDraught(enfoncement float64, offsetFlotteur float64) float64 {
	return enfoncement + offsetFlotteur
}

func CalculateOrganeanDepth(enfoncement float64, offsetOrganeau float64) float64 {
	return -(enfoncement + offsetOrganeau)
}

func CalculateMaxDepth(depth float64, marnage float64, waveHeight float64) float64 {
	return depth + marnage + (waveHeight / 2.0)
}

func CalculateMinDepth(depth float64, waveHeight float64) float64 {
	return depth - (waveHeight / 2.0)
}
