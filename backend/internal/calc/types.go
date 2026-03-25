package calc

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
}
