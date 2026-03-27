package calc

import "backend/internal/models"

// SiteConditions regroupe les paramètres météo-océaniques du site
type SiteConditions struct {
	WaterDepth      float64 `json:"water_depth"`      // Profondeur moyenne (m)
	WaterDensity    float64 `json:"water_density"`    // Densité (kg/m3) - Default: 1025
	Marnage         float64 `json:"marnage"`           // Différence marée haute/basse (m)
	WindVelocity    float64 `json:"wind_velocity"`    // Vitesse du vent (m/s)
	CurrentVelocity float64 `json:"current_velocity"` // Vitesse courant (m/s)
	WaveHeight      float64 `json:"wave_height"`      // Hauteur de houle (m)
	WavePeriod      float64 `json:"wave_period"`      // Période de houle (s)
}

// SimulationParams regroupe les objets et choix de l'utilisateur
type SimulationParams struct {
	Buoy               models.Buoy
	Chain              models.Chain
	NumBallast         int
	ExtraEquipmentMass float64
	LestDensity        float64 // kg/m³
	AnchorDensity      float64 // kg/m³ (2400 pour béton, 7850 pour acier)
	ChainQualityCharge float64 // Charge d'épreuve en tonnes (Q1, Q2 ou Q3)
}

// SimulationResult contient toutes les données de sortie du moteur
type SimulationResult struct {
	// --- Résultats Principaux ---
	Enfoncement             float64 `json:"enfoncement"`               // Mètres
	TensionMaxMouillage     float64 `json:"tension_max_mouillage"`   // Tonnes
	RayonEvitage            float64 `json:"rayon_evitage"`           // Mètres
	FrancBord               float64 `json:"franc_bord"`               // Mètres
	ReserveFlottabilite     float64 `json:"reserve_flottabilite"`     // %
	TirantEau               float64 `json:"tirant_eau"`               // Mètres
	
	// --- Sécurité ---
	IsSafe                  bool    `json:"is_safe"`                   // Cohérence globale
	CoefSecuriteChaine      float64 `json:"coef_securite_chaine"`
	MasseMinCorpsMort       float64 `json:"masse_min_corps_mort"`     // kg
	
	// --- Données d'Entrées Appliquées ---
	BuoyName                string         `json:"buoy_name"`
	ChainType               string         `json:"chain_type"`
	SiteConditions          SiteConditions `json:"site_conditions"`
	
	// --- Détails Physiques ---
	TraineeVent             float64 `json:"trainee_vent"`             // kg
	TraineeCourant          float64 `json:"trainee_courant"`          // kg
	TraineeVague            float64 `json:"trainee_vague"`            // kg
	VitesseCourantSurface   float64 `json:"vitesse_courant_surface"`   // m/s
	TimeCalcul              float64 `json:"time_calcul"`              // ms

	// --- Nouveaux champs pour Fidélité 100% ---
	LongueurCatenary        float64 `json:"longueur_catenary"`        // m
	SurfaceLateraleEmergee  float64 `json:"surface_laterale_emergee"`  // m2
	SurfaceLateraleImmergee float64 `json:"surface_laterale_immergee"` // m2
	PoidsLineiqueImmerge    float64 `json:"poids_lineique_immerge"`    // kg/m
	EffortHorizontalKg      float64 `json:"effort_horizontal_kg"`      // kg
	PoidsLestImmerge        float64 `json:"poids_lest_immerge"`        // kg
	MasseBoueeFixe          float64 `json:"masse_bouee_fixe"`          // kg (Avec équipements)
}
