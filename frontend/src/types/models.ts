export interface BuoyElement {
  H: number;
  D0: number;
  D1: number;
}

export interface BuoyComponent {
  Name: string;
  Elements?: BuoyElement[];
  Height?: number;
  WidthHigh?: number;
  WidthLow?: number;
  OffsetFlotteur?: number;
  OffsetOrganeau?: number;
}

export interface Buoy {
  id: number;
  name: string;
  chaine_min: number;
  chaine_max: number;
  structure?: BuoyComponent;
  flotteur?: BuoyComponent;
  pylone?: BuoyComponent[];
  equipement?: BuoyComponent[];
}

export interface Chain {
  id: number;
  dn: number;
  type: string;
}

export interface SiteConditions {
  water_depth: number;
  marnage: number;
  wave_height: number;
  wave_significant: number;
  wave_period: number;
  wind_velocity: number;
  current_velocity: number;
  water_density: number;
}
