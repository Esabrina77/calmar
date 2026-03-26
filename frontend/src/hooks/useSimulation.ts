"use client";

import { useState, useEffect } from "react";
import { fetchBuoys, fetchChains, fetchBuoyData, simulateBuoy } from "../api/buoyService";
import { Buoy, Chain, SiteConditions } from "../types/models";

export function useSimulation() {
  const [buoys, setBuoys] = useState<Buoy[]>([]);
  const [chains, setChains] = useState<Chain[]>([]);

  // Saisie Utilisateur
  const [selectedBuoy, setSelectedBuoy] = useState<number>(0);
  const [selectedBuoyData, setSelectedBuoyData] = useState<any>(null);
  const [selectedChain, setSelectedChain] = useState<number>(0);
  const [chainQuality, setChainQuality] = useState("Q1");
  const [anchorType, setAnchorType] = useState("Béton"); // 2400 ou 7850

  const [site, setSite] = useState<SiteConditions>({
    water_depth: 10,
    marnage: 2,
    wind_velocity: 15,
    current_velocity: 0.5,
    wave_height: 1.5, // H_max
    wave_significant: 0.81, // H_s
    water_density: 1025, // 1000 ou 1025
  });

  const [mooring, setMooring] = useState({
    chain_length: 30,
    target_safety: 3.0,
  });

  const [equipment, setEquipment] = useState({
    num_ballast: 1,
    mass_equipment: 50,
  });

  const [result, setResult] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchBuoys().then(setBuoys).catch(console.error);
    fetchChains().then(setChains).catch(console.error);
  }, []);

  useEffect(() => {
    if (selectedBuoy > 0) {
      fetchBuoyData(selectedBuoy).then(setSelectedBuoyData).catch(console.error);
    }
  }, [selectedBuoy]);

  // Calcul automatique Hs
  const updateWaveHeight = (hmax: number) => {
    const hs = hmax / 1.8667;
    setSite({
      ...site,
      wave_height: hmax,
      wave_significant: Number(hs.toFixed(2)),
    });
  };

  const handleSimulate = async () => {
    setLoading(true);
    try {
      const data = await simulateBuoy({
        buoy_id: Number(selectedBuoy),
        chain_id: Number(selectedChain),
        num_ballast: equipment.num_ballast,
        equipment_mass: equipment.mass_equipment,
        chain_quality: chainQuality,
        anchor_density: anchorType === "Béton" ? 2400 : 7850,
        conditions: {
          ...site,
          marnage: Number(site.marnage),
          wave_height: Number(site.wave_height),
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
    chains,
    selectedBuoy,
    setSelectedBuoy,
    selectedBuoyData,
    selectedChain,
    setSelectedChain,
    chainQuality,
    setChainQuality,
    anchorType,
    setAnchorType,
    site,
    setSite,
    updateWaveHeight,
    mooring,
    setMooring,
    equipment,
    setEquipment,
    result,
    loading,
    handleSimulate,
  };
}
