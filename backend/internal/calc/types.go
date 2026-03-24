package calc

import "time"

// SimulationResult (équivalent à STR_RET_CALCUL_RESULTAT en VB.NET)
// Contient toutes les conclusions et les statistiques de la simulation d'ancrage
type SimulationResult struct {
	// --- Resultats Globaux ---
	ProfondeurMax        float64 `json:"profondeur_max"`        // PROFONDEUR_MAX (Mètres)
	LongueurCatenaire    float64 `json:"longueur_catenaire"`    // LONGUEUR_CATENAIRE (Mètres)
	TensionMaxMouillage  float64 `json:"tension_max_mouillage"` // TENSION_CHAINE (Tonnes ou kg selon unité)
	MasseMinimaleCM      float64 `json:"masse_minimale_cm"`      // MASSE_MIN_CORPS_MORT (kg)
	ReserveFlotabilite   float64 `json:"reserve_flotabilite"`   // RESERVE_FLOATABILITE (%)
	FrancBordBouee       float64 `json:"franc_bord_bouee"`       // FRANC_BORD (Mètres)
	SurfaceLateraleEmergee float64 `json:"surface_laterale_emergee"` // SURFACE_EMERGEE (m²)
	ProfondeurMin        float64 `json:"profondeur_min"`        // PROFONDEUR_MIN (Mètres)
	RayonEvitage         float64 `json:"rayon_evitage"`         // RAYON_D_EVITAGE (Mètres)
	CoefSecChaine        float64 `json:"coef_sec_chaine"`        // COEFFICIENT_SECURITE_CHAINE
	CoefSecCM            float64 `json:"coef_sec_cm"`            // Rapport Poids CM Immerge / EffortHorizontal
	DeplacementTotal     float64 `json:"deplacement_total"`     // DEPLACEMENT_TOTAL (Tonnes)
	TirantEau            float64 `json:"tirant_eau"`            // TIRANT_D_EAU (Mètres)
	SurfaceLateraleImmergee float64 `json:"surface_laterale_immergee"` // SURFACE_IMMERGEE (m²)

	// --- Données d'Entrées Appliquées ---
	DiametreChaine       float64 `json:"diametre_chaine"` // DN (Mètres)
	ChainType            string  `json:"chain_type"`      // Ex: "DN26 Q2"
	Profondeur           float64 `json:"profondeur"`      // PROFONDEUR initial (Mètres)
	MasseEquipement      float64 `json:"masse_equipement"` // kg
	Marnage              float64 `json:"marnage"`         // Mètres
	DensiteCM            float64 `json:"densite_cm"`      // kg/m³
	PeriodeVague         float64 `json:"periode_vague"`   // Secondes
	VitMaxCourantMS      float64 `json:"vit_max_courant_ms"` // m/s
	VitMaxCourantNDS     float64 `json:"vit_max_courant_nds"` // Noeuds
	HauteurMaxVague      float64 `json:"hauteur_max_vague"` // Mètres
	HauteurSignifVague   float64 `json:"hauteur_signif_vague"` // Mètres
	VitMaxVentMS         float64 `json:"vit_max_vent_ms"` // m/s
	VitMaxVentKMH        float64 `json:"vit_max_vent_kmh"` // km/h

	// --- Variables Intermédiaires du Moteur ---
	Enfoncement          float64 `json:"enfoncement"`           // Équivalent VB _BUOY.FlotteurBouee.HAUTEUR_IMMERGEE
	MasseBouee           float64 `json:"masse_bouee"`           // kg
	HauteurCatenaire     float64 `json:"hauteur_catenaire"`     // Metres
	VolumeDeplacer       float64 `json:"volume_deplacer"`       // m3
	EffortHorizontalKg   float64 `json:"effort_horizontal_kg"`  // kg
	PoidsLineiqueImmergeChaine float64 `json:"poids_lineique_immerge_chaine"` // kg/m
	PoidsLestImmerge     float64 `json:"poids_lest_immerge"`     // kg
	AngleTangence        float64 `json:"angle_tangence"`        // Degrés (°)

	// --- Détails des Efforts de Traînées (Drag) ---
	TraineeVent          float64 `json:"trainee_vent"`           // t
	TraineeCourant       float64 `json:"trainee_courant"`        // t
	TraineeVague         float64 `json:"trainee_vague"`          // t
	VitesseCourantSurface float64 `json:"vitesse_courant_surface"` // m/s

	// --- Métadonnées ---
	TimeCalcul           time.Duration `json:"time_calcul"` // Temps CPU de la boucle
}
