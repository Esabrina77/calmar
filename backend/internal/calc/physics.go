package calc

import "math"

// --- Équivalents VB.NET Paramètres par Défaut ---
const (
	WaterDensity      = 1.025   // DensiteEau (kg/m3 pour eau salée, ou 1.025 t/m3)
	AirDensity        = 0.00129 // DensiteAir
	MetalDensity      = 7.85    // DensiteMetal (Acier)
	BallastDensity    = 7.32    // DensiteLest (Fonte)
	Gravity           = 9.81    // Constante gravitationnelle (m/s²)
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

