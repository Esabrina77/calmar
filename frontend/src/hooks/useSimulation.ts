"use client";

import { useState, useEffect } from "react";
import { fetchBuoys, fetchChains, fetchBuoyData, simulateBuoy, fetchEquipments } from "../api/buoyService";
import { Buoy, Chain, SiteConditions } from "../types/models";

export function useSimulation() {
  const [buoys, setBuoys] = useState<Buoy[]>([]);
  const [chains, setChains] = useState<Chain[]>([]);
  const [equipmentStandards, setEquipmentStandards] = useState<any[]>([]);
  const [selectedEquipments, setSelectedEquipments] = useState<Record<number, number>>({});

  // Saisie Utilisateur
  const [selectedBuoy, setSelectedBuoy] = useState<number>(0);
  const [selectedBuoyData, setSelectedBuoyData] = useState<any>(null);

  // LIGNE DE MOUILLAGE (Logique .NET)
  const [selectedDN, setSelectedDN] = useState<number>(0);
  const [selectedType, setSelectedType] = useState<string>("");
  const [selectedChain, setSelectedChain] = useState<number>(0);
  const [chainQuality, setChainQuality] = useState<number>(1); // 1, 2, 3

  // UNITÉS
  const [windUnit, setWindUnit] = useState<"m/s" | "km/h">("km/h");
  const [currentUnit, setCurrentUnit] = useState<"m/s" | "kn">("kn");

  const [site, setSite] = useState<SiteConditions>({
    water_depth: 10,
    marnage: 2,
    wind_velocity: 15, // Toujours stocké en m/s en interne pour le moteur
    current_velocity: 0.5, // Toujours stocké en m/s en interne
    wave_height: 1.5, // H_max
    wave_significant: 0.81, // H_s
    wave_period: 4.0, // Valeur par défaut réaliste
    water_density: 1025,
  });

  const [anchorDensity, setAnchorDensity] = useState<number>(2.4); // Béton std


  const [mooring, setMooring] = useState({
    chain_length: 30,
    target_safety: 3.0,
  });

  const [equipment, setEquipment] = useState({
    num_ballast: 1,
  });

  // Calcul automatique de la masse des équipements sélectionnés
  const totalEquipmentMass = Object.entries(selectedEquipments).reduce((acc, [id, count]) => {
    const eq = equipmentStandards.find(e => e.id === Number(id));
    return acc + (eq ? eq.masse_unitaire * count : 0);
  }, 0);

  const [result, setResult] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchBuoys().then(setBuoys).catch(console.error);
    fetchChains().then(setChains).catch(console.error);
    fetchEquipments().then(setEquipmentStandards).catch(console.error);
  }, []);

  useEffect(() => {
    if (selectedBuoy > 0) {
      fetchBuoyData(selectedBuoy).then(setSelectedBuoyData).catch(console.error);
      // Reset chain selection when buoy changes
      setSelectedDN(0);
      setSelectedType("");
      setSelectedChain(0);
    }
  }, [selectedBuoy]);

  // Filtrage des DN disponibles (Provenant du Backend)
  const availableDNs = Array.from(
    new Set(
      chains
        .filter((c) => {
          if (!selectedBuoyData) return true;
          return (
            c.dn >= selectedBuoyData.chaine_min &&
            c.dn <= selectedBuoyData.chaine_max
          );
        })
        .map((c) => c.dn)
    )
  ).sort((a, b) => a - b);

  // Filtrage des Types disponibles pour un DN (Provenant du Backend)
  const availableTypes = Array.from(
    new Set(
      chains.filter((c) => c.dn === selectedDN).map((c) => c.type)
    )
  ).sort();

  // Liste des Lests disponibles (Suggéré par la bouée)
  const availableBallasts = selectedBuoyData
    ? Array.from(
        { length: selectedBuoyData.nombre_lest_max - selectedBuoyData.nombre_lest_min + 1 },
        (_, i) => selectedBuoyData.nombre_lest_min + i
      )
    : [0];

  // Mise à jour de la chaîne finale
  useEffect(() => {
    const chain = chains.find(
      (c) => c.dn === selectedDN && c.type === selectedType
    );
    if (chain) {
      setSelectedChain(chain.id);
    } else {
      setSelectedChain(0);
    }
  }, [selectedDN, selectedType, chains]);

  // Synchronisation des Vagues (Hmax = 1.85 * Hs)
  const updateWaveSignificant = (hs: number) => {
    setSite({
      ...site,
      wave_significant: hs,
      wave_height: Number((hs * 1.85).toFixed(2)),
    });
  };

  const updateWaveMax = (hmax: number) => {
    setSite({
      ...site,
      wave_height: hmax,
      wave_significant: Number((hmax / 1.85).toFixed(2)),
    });
  };

  // Conversions Display -> Interne (m/s)
  const setWindDisplay = (val: number) => {
    const internal = windUnit === "km/h" ? val / 3.6 : val;
    setSite({ ...site, wind_velocity: internal });
  };

  const setCurrentDisplay = (val: number) => {
    const internal = currentUnit === "kn" ? (val * 1.852) / 3.6 : val;
    setSite({ ...site, current_velocity: internal });
  };

  const handleSimulate = async () => {
    if (!selectedChain) {
      alert("Veuillez sélectionner une chaîne (DN et Type).");
      return;
    }
    setLoading(true);
    try {
      const data = await simulateBuoy({
        buoy_id: Number(selectedBuoy),
        chain_id: Number(selectedChain),
        num_ballast: equipment.num_ballast,
        equipment_mass: totalEquipmentMass,
        chain_quality: `Q${chainQuality}`,
        anchor_density: anchorDensity * 1000, // Conversion t/m3 en kg/m3
        conditions: {
          ...site,
          marnage: Number(site.marnage),
          wave_height: Number(site.wave_height),
          wave_period: Number(site.wave_period),
        },
      });
      setResult(data);
    } catch (err) {
      console.error(err);
      alert("Erreur lors de la simulation");
    } finally {
      setLoading(false);
    }
  };

  return {
    buoys,
    availableDNs,
    availableTypes,
    availableBallasts,
    selectedDN,
    setSelectedDN,
    selectedType,
    setSelectedType,
    selectedBuoy,
    setSelectedBuoy,
    selectedBuoyData,
    selectedChain,
    setSelectedChain,
    chainQuality,
    setChainQuality,
    anchorDensity,
    setAnchorDensity,
    site,
    setSite,
    windUnit,
    setWindUnit,
    currentUnit,
    setCurrentUnit,
    setWindDisplay,
    setCurrentDisplay,
    updateWaveSignificant,
    updateWaveMax,
    mooring,
    setMooring,
    equipment,
    setEquipment,
    equipmentStandards,
    selectedEquipments,
    setSelectedEquipments,
    totalEquipmentMass,
    result,
    loading,
    handleSimulate,
  };
}
