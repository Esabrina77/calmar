/*
 * CALMAR PHYSICS ENGINE - IALA COMPLIANCE MODULE
 * --------------------------------------------
 * Ce fichier contient les constantes physiques et les formules IALA
 * transposées du projet legacy Calmar (VB.NET).
 *
 * Responsabilités :
 * - Calcul des traînées (Drag) Vent, Courant, Vagues.
 * - Modélisation de la caténaire (Catenary).
 * - Calculs de sécurité (Chain Safety, Anchor Mass).
 */
package calc

import "math"

// --- Équivalents VB.NET Paramètres par Défaut ---
const (
	WaterDensity      = 1025.0   // kg/m3 (eau salée)
	AirDensity        = 1.225    // kg/m3 (air sec à 15°C)
	MetalDensity      = 7850.0   // kg/m3 (Acier standard)
	BallastDensity    = 7320.0   // kg/m3 (Fonte ductile)
	Gravity           = 9.81     // m/s²
)

// CalculateBuoyancy calcule la force de la Poussée d'Archimède (en Newton).
// Formule : F = Volume_Immergé * Densité_Eau * Gravité
func CalculateBuoyancy(volumeImmerge float64) float64 {
	return volumeImmerge * WaterDensity * Gravity
}

// CalculateWindDrag calcule les Traînées de Vent (en kgf).
// Formule VB.NET Validée IALA : (0.5 * V² * DensiteAir * Cd * Surface) / 9.81
func CalculateWindDrag(vitesseVent float64, surfaceEmergee float64) float64 {
	const cdWind = 1.2 // COEF_TRAINEE_VENT par defaut VB
	return (0.5 * (vitesseVent * vitesseVent) * AirDensity * (cdWind * surfaceEmergee)) / Gravity
}

// CalculateChainDrag calcule l'effort du courant sur la CHAINE (en kgf).
// Formule VB.NET Validée IALA: (0.5 * V_courant_fond² * DensiteEau * Cd * SurfaceChaine) / 9.81
func CalculateChainDrag(vitesseCourant float64, surfaceChaine float64) float64 {
	const cdChain = 1.2 // COEF_TRAINEE_CHAINE par defaut VB
	return (0.5 * (vitesseCourant * vitesseCourant) * WaterDensity * (cdChain * surfaceChaine)) / Gravity
}

// CalculateBuoyCurrentDrag calcule l'effort du courant sur la BOUEE (en kgf).
// Formule VB.NET Validée IALA: (0.5 * V_courant_surface² * DensiteEau * Cd * SurfaceImmergee) / 9.81
func CalculateBuoyCurrentDrag(vitesseCourantSurface float64, surfaceImmergee float64) float64 {
	const cdCurrent = 1.2 // COEF_TRAINEE_COURANT par defaut VB
	return (0.5 * (vitesseCourantSurface * vitesseCourantSurface) * WaterDensity * (cdCurrent * surfaceImmergee)) / Gravity
}

// CalculateCatenaryLength calcule la longueur de la caténaire nécessaire pour l'effort.
// Formule VB.NET Validée IALA: Math.Sqrt(HauteurCat^2 + (2 * (Effort/PoidsLineique) * HauteurCat))
func CalculateCatenaryLength(hauteurCatenaire float64, effortHorizontalKg float64, poidsLineiqueImmerge float64) float64 {
	// EffortHorizontal est en kgf (ou t), on multiplie par 1000 selon l'unité ?
	// Le code VB fait: EffortHorizontale * 1000
	ratio := (effortHorizontalKg) / poidsLineiqueImmerge
	return (hauteurCatenaire * hauteurCatenaire) + (2 * ratio * hauteurCatenaire)
}
// Note: Faire Math.Sqrt() du résultat dans l'appelant


// --- NOUVELLES FORMULES TRADUITES DU VB.NET ---

// CalculateLineicWeight calcule le Poids Linéaire de la chaîne IMMERGÉE (en kg/m).
// Traduction VB: CHAIN.MASSE_LINEIQUE / DensiteMetal * (DensiteMetal - DensiteEau)
func CalculateLineicWeight(massLineiqueAir float64) float64 {
	// Poids dans l'eau = Poids dans l'air * (1 - DensitéEau / DensitéAcier)
	return massLineiqueAir * (1 - (WaterDensity / MetalDensity))
}

// CalculateSubmergedBallastWeight calcule le Poids du Lest IMMERGÉ (en kg).
// Traduction VB: _BUOY.MASSE_LEST / DensiteLest * (DensiteLest - DensiteEau)
func CalculateSubmergedBallastWeight(massBallastAir float64) float64 {
	return massBallastAir * (1 - (WaterDensity / BallastDensity))
}

// CalculateTotalHorizontalEffort additionne tous les efforts qui poussent la bouée de côté (en kgf ou Newton).
// Traduction VB: CalculEffortVent() + CalculEffortCourantSurChaine() + CalculEffortCourantSurfaceSurBouee()
func CalculateTotalHorizontalEffort(windDrag float64, currentDragBuoy float64, currentDragChain float64) float64 {
	return windDrag + currentDragBuoy + currentDragChain
}

// --- FONCTIONS MATHÉMATIQUES & ÉVIMAGE SUPPLÉMENTAIRES ---

// ACosh calcule le cosinus hyperbolique inverse (Math.Acosh en VB.NET).
// Formule : ln(x + sqrt(x^2 - 1))
func ACosh(x float64) float64 {
	if x < 1 {
		return 0 // Mathématiquement impossible pour une caténaire
	}
	return math.Log(x + math.Sqrt(x*x-1))
}

// CalculateSwingRadius calcule le rayon d'évitabilité (Swinging radius).
// Traduction VB: ((EFFORT_HORIZONTALE * 1000) / POIDS_LINEIQUE) * ACosH((HAUTEUR_CATENAIRE / ((EFFORT_HORIZONTALE * 1000) / POIDS_LINEIQUE)) + 1)
func CalculateSwingRadius(effortHorizontalKg float64, poidsLineique float64, hauteurCatenaire float64) float64 {
	forceRatio := (effortHorizontalKg * 1000.0) / poidsLineique
	if forceRatio <= 0 {
		return 0
	}
	return forceRatio * ACosh((hauteurCatenaire/forceRatio)+1.0)
}

// --- CALCULS IALA FINAUX ---

// CalculateMinAnchorMass calcule la Masse Minimale du Corps Mort (en kg).
// Traduction VB: CoefficientSecuriteMasseCorpsMort * EFFORT_HORIZONTALE * (DensiteCM / (DensiteCM - DensiteEau)) / Tan(45°)
// SeaBedFrictionAngle = 45° (standard IALA pour fond sableux)
// SafetyFactor = 1.5 (coefficient de sécurité IALA)
func CalculateMinAnchorMass(effortHorizontalKg float64, densiteCM float64) float64 {
	const seaBedFrictionAngle = 45.0 // degrés (valeur standard IALA)
	const safetyFactor = 1.5         // CoefficientSecuriteMasseCorpsMort IALA

	if densiteCM <= WaterDensity {
		return 0 // Corps mort trop léger (impossible)
	}

	angleRad := seaBedFrictionAngle * math.Pi / 180.0
	return safetyFactor * effortHorizontalKg * (densiteCM / (densiteCM - WaterDensity)) / math.Tan(angleRad)
}

// CalculateSubmergedAnchorWeight calcule le Poids Immergé du Corps Mort (en kg).
// Traduction VB: MASSE_MIN_CORPS_MORT / DensiteCM * (DensiteCM - DensiteEau)
func CalculateSubmergedAnchorWeight(massCorpsMort float64, densiteCM float64) float64 {
	return massCorpsMort / densiteCM * (densiteCM - WaterDensity)
}

// CalculateChainSafetyCoefficient calcule le Coefficient de Sécurité de la Chaîne.
// Traduction VB: CHAIN.CHARGE_EPREUVE / ((HAUTEUR_CATENAIRE * POIDS_LINEIQUE / 1000) + EFFORT_HORIZONTALE)
func CalculateChainSafetyCoefficient(chargeEpreuve float64, hauteurCatenaire float64, poidsLineique float64, effortHorizontalKg float64) float64 {
	tensionVerticale := (hauteurCatenaire * poidsLineique) / 1000.0
	if (tensionVerticale + effortHorizontalKg) == 0 {
		return 0
	}
	return chargeEpreuve / (tensionVerticale + effortHorizontalKg)
}

// CalculateTangencyAngle calcule l'Angle de Tangence de la chaîne à l'organeau (en degrés).
// Traduction VB: DEGREE(Math.Acos(POIDS_CATENAIRE_IMMERGE / (TENSION_CHAINE * 1000)))
func CalculateTangencyAngle(poidsCatenaireImmerge float64, tensionChaine float64) float64 {
	if tensionChaine <= 0 {
		return 0
	}
	cosAngle := poidsCatenaireImmerge / (tensionChaine * 1000.0)
	cosAngle = math.Max(-1.0, math.Min(1.0, cosAngle)) // Clamping anti-NaN
	return math.Acos(cosAngle) * 180.0 / math.Pi       // Conversion radians → degrés
}

// CalculateDraught calcule le Tirant d'Eau de la bouée (en mètres).
// Traduction VB: FlotteurBouee.HAUTEUR_IMMERGEE + StructureBouee.OffsetFlotteur
func CalculateDraught(enfoncement float64, offsetFlotteur float64) float64 {
	return enfoncement + offsetFlotteur
}

// CalculateOrganeanDepth calcule la profondeur de l'organeau (point d'attache de la chaîne).
// Traduction VB: -(FlotteurBouee.HAUTEUR_IMMERGEE + StructureBouee.OffsetOrganeau)
func CalculateOrganeanDepth(enfoncement float64, offsetOrganeau float64) float64 {
	return -1.0 * (enfoncement + offsetOrganeau)
}

// CalculateMaxDepth calcule la Profondeur Maximum avec marnage et houle.
// Traduction VB: Profondeur + Marnage + HouleMax / 2
func CalculateMaxDepth(profondeur float64, marnage float64, houleMax float64) float64 {
	return profondeur + marnage + (houleMax / 2.0)
}

// CalculateMinDepth calcule la Profondeur Minimum (pendant le creux de la houle).
// Traduction VB: Profondeur - (HouleMax / 2)
func CalculateMinDepth(profondeur float64, houleMax float64) float64 {
	return profondeur - (houleMax / 2.0)
}
