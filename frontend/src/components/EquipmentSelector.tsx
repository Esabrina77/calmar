"use client";

import React from "react";

interface Equipment {
  id: number;
  categorie: string;
  name: string;
  masse_unitaire: number;
}

interface EquipmentSelectorProps {
  catalog: Equipment[];
  selected: Record<number, number>;
  onChange: (selected: Record<number, number>) => void;
  totalMass: number;
}

export default function EquipmentSelector({
  catalog,
  selected,
  onChange,
  totalMass,
}: EquipmentSelectorProps) {
  // Sécurité si catalog n'est pas encore chargé ou n'est pas un tableau
  const safeCatalog = Array.isArray(catalog) ? catalog : [];

  // Grouper le catalogue par catégorie
  const groupedCatalog = safeCatalog.reduce((acc, eq) => {
    if (!acc[eq.categorie]) acc[eq.categorie] = [];
    acc[eq.categorie].push(eq);
    return acc;
  }, {} as Record<string, Equipment[]>);

  const toggleEquipment = (id: number, delta: number) => {
    const current = selected[id] || 0;
    const next = Math.max(0, current + delta);
    const newSelected = { ...selected };
    if (next === 0) {
      delete newSelected[id];
    } else {
      newSelected[id] = next;
    }
    onChange(newSelected);
  };

  return (
    <div className="glass-card" style={{ padding: "1.5rem", height: "100%", display: "flex", flexDirection: "column" }}>
      <h3 className="section-title">
        <span>📦 Équipements Additionnels</span>
      </h3>

      <div style={{ flex: 1, overflowY: "auto", marginBottom: "1rem", paddingRight: "0.5rem" }}>
        {Object.entries(groupedCatalog).map(([category, items]) => (
          <div key={category} style={{ marginBottom: "1.5rem" }}>
            <h4 style={{ fontSize: "0.85rem", color: "var(--text-secondary)", marginBottom: "0.5rem", borderBottom: "1px solid var(--border)", paddingBottom: "0.25rem" }}>
              {category}
            </h4>
            <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
              {items.map((item) => (
                <div
                  key={item.id}
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    padding: "0.5rem",
                    borderRadius: "var(--radius-sm)",
                    background: selected[item.id] ? "var(--primary-light)" : "transparent",
                    border: "1px solid var(--border)",
                    fontSize: "0.85rem"
                  }}
                >
                  <div style={{ flex: 1 }}>
                    <div style={{ fontWeight: 500 }}>{item.name}</div>
                    <div style={{ fontSize: "0.75rem", color: "var(--text-muted)" }}>{item.masse_unitaire} kg</div>
                  </div>
                  
                  <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
                    {selected[item.id] && (
                      <span style={{ fontWeight: 700, color: "var(--primary)" }}>{selected[item.id]}x</span>
                    )}
                    <div style={{ display: "flex", gap: "0.25rem" }}>
                      <button
                        onClick={() => toggleEquipment(item.id, -1)}
                        className="btn"
                        style={{ padding: "0.2rem 0.6rem", fontSize: "0.8rem", minWidth: "30px" }}
                      >
                        -
                      </button>
                      <button
                        onClick={() => toggleEquipment(item.id, 1)}
                        className="btn btn-primary"
                        style={{ padding: "0.2rem 0.6rem", fontSize: "0.8rem", minWidth: "30px" }}
                      >
                        +
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      <div style={{ 
        marginTop: "auto", 
        padding: "1rem", 
        background: "var(--primary)", 
        color: "var(--primary-contrast)", 
        borderRadius: "var(--radius-md)",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center"
      }}>
        <span style={{ fontSize: "0.9rem", fontWeight: 600 }}>Masse Totale</span>
        <span style={{ fontSize: "1.1rem", fontWeight: 800 }}>{totalMass.toFixed(2)} kg</span>
      </div>
    </div>
  );
}
