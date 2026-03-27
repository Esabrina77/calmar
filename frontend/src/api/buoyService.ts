import { Buoy, Chain } from '../types/models';

const API_URL = "http://localhost:4000/api";

export const fetchBuoys = async (): Promise<Buoy[]> => {
  const res = await fetch(`${API_URL}/buoys`);
  return res.json();
};

export const fetchChains = async (): Promise<Chain[]> => {
  const res = await fetch(`${API_URL}/chains`);
  return res.json();
};

export const fetchEquipments = async (): Promise<any[]> => {
  const res = await fetch(`${API_URL}/equipments`);
  return res.json();
};

export const fetchBuoyData = async (id: number): Promise<any> => {
  const res = await fetch(`${API_URL}/buoys/${id}`);
  return res.json();
};

export const simulateBuoy = async (payload: any): Promise<any> => {
  const res = await fetch(`${API_URL}/simulate`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  return res.json();
};
