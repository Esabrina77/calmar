"use client";

import React from "react";
import dynamic from "next/dynamic";
import { useSimulation } from "../hooks/useSimulation";
import BuoyRenderer from "./BuoyRenderer";
import EquipmentSelector from "./EquipmentSelector";

// SimulationDashboard.tsx - Premium Refurbished Version v2.2 (Modal + Layout Fix)
export default function SimulationDashboard() {
  const {
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
  } = useSimulation();

  const [searchTerm, setSearchTerm] = React.useState("");
  const [currentPage, setCurrentPage] = React.useState(1);
  const [isEquipModalOpen, setIsEquipModalOpen] = React.useState(false);
  
  const itemsPerPage = 8;

  const filteredBuoys = buoys.filter((b: any) =>
    b.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const totalPages = Math.ceil(filteredBuoys.length / itemsPerPage);
  const paginatedBuoys = filteredBuoys.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage,
  );

  const selectedEquipCount = Object.values(selectedEquipments).reduce((a, b) => a + b, 0);

  return (
    <div className="animate-fade-in" style={{ padding: "0 1rem 4rem 1rem" }}>
      

      <div className="grid-cols-12">
        {/* LEFT COLUMN: SETUP & CONFIG (8 cols) */}
        <div className="col-span-8" style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
          
          {/* SÉLECTEUR DE BOUÉES */}
          <section className="glass-card" style={{ padding: "1.5rem" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1.5rem" }}>
              <h3 className="section-title"><span>⚓ Sélection du Modèle</span></h3>
              <input
                type="text"
                placeholder="Rechercher..."
                className="premium-input"
                style={{ width: "250px" }}
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value);
                  setCurrentPage(1);
                }}
              />
            </div>
            
            <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: "1rem" }}>
              {paginatedBuoys.map((b: any) => (
                <button
                  key={b.id}
                  onClick={() => setSelectedBuoy(b.id)}
                  style={{
                    padding: "1.25rem 1rem",
                    borderRadius: "var(--radius-md)",
                    background: selectedBuoy === b.id ? "var(--primary)" : "var(--surface-muted)",
                    color: selectedBuoy === b.id ? "white" : "inherit",
                    border: selectedBuoy === b.id ? "1px solid var(--primary)" : "1px solid var(--border)",
                    cursor: "pointer",
                    textAlign: "center",
                    transition: "all 0.2s cubic-bezier(0.4, 0, 0.2, 1)",
                    transform: selectedBuoy === b.id ? "translateY(-2px)" : "none"
                  }}
                >
                  <div style={{ fontWeight: 800, fontSize: "0.9rem" }}>{b.name}</div>
                </button>
              ))}
            </div>

            {totalPages > 1 && (
              <div style={{ marginTop: "1rem", display: "flex", gap: "0.5rem", justifyContent: "center" }}>
                {Array.from({ length: totalPages }).map((_, i) => (
                  <button
                    key={i}
                    onClick={() => setCurrentPage(i + 1)}
                    style={{
                      width: "30px", height: "30px", borderRadius: "8px",
                      background: currentPage === i + 1 ? "var(--primary)" : "transparent",
                      color: currentPage === i + 1 ? "white" : "var(--text-muted)",
                      border: "1px solid var(--border)", cursor: "pointer",
                      fontSize: "0.75rem", fontWeight: 700
                    }}
                  >
                    {i + 1}
                  </button>
                ))}
              </div>
            )}
          </section>

          {/* SITE CONDITIONS & MOORING */}
          <div className="grid-cols-12">
            <section className="glass-card col-span-6" style={{ padding: "1.5rem" }}>
              <h3 className="section-title"><span>🌊 Environnement</span></h3>
              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.25rem" }}>
                <div>
                  <label className="label-modern">Profondeur (m)</label>
                  <input type="number" className="premium-input" value={site.water_depth} onChange={(e) => setSite({ ...site, water_depth: Number(e.target.value) })} />
                </div>
                <div>
                  <label className="label-modern">Marnage (m)</label>
                  <input type="number" className="premium-input" value={site.marnage} onChange={(e) => setSite({ ...site, marnage: Number(e.target.value) })} />
                </div>
                <div>
                  <label className="label-modern">Vent ({windUnit})</label>
                  <div style={{ display: "flex", gap: "0.25rem", marginBottom: "0.4rem" }}>
                    <button onClick={() => setWindUnit("m/s")} style={{ ...miniBtnStyle(windUnit === "m/s") }}>m/s</button>
                    <button onClick={() => setWindUnit("km/h")} style={{ ...miniBtnStyle(windUnit === "km/h") }}>km/h</button>
                  </div>
                  <input type="number" className="premium-input" value={windUnit === "km/h" ? Number((site.wind_velocity * 3.6).toFixed(1)) : Number(site.wind_velocity.toFixed(1))} onChange={(e) => setWindDisplay(Number(e.target.value))} />
                </div>
                <div>
                  <label className="label-modern">Courant ({currentUnit})</label>
                  <div style={{ display: "flex", gap: "0.25rem", marginBottom: "0.4rem" }}>
                    <button onClick={() => setCurrentUnit("m/s")} style={{ ...miniBtnStyle(currentUnit === "m/s") }}>m/s</button>
                    <button onClick={() => setCurrentUnit("kn")} style={{ ...miniBtnStyle(currentUnit === "kn") }}>kn</button>
                  </div>
                  <input type="number" className="premium-input" value={currentUnit === "kn" ? Number(((site.current_velocity * 3.6) / 1.852).toFixed(2)) : Number(site.current_velocity.toFixed(2))} onChange={(e) => setCurrentDisplay(Number(e.target.value))} />
                </div>
                <div>
                  <label className="label-modern">Hs (m)</label>
                  <input type="number" step="0.1" className="premium-input" value={site.wave_significant} onChange={(e) => updateWaveSignificant(Number(e.target.value))} />
                </div>
                <div>
                  <label className="label-modern">Hmax (m)</label>
                  <input type="number" step="0.1" className="premium-input" value={site.wave_height} onChange={(e) => updateWaveMax(Number(e.target.value))} />
                </div>
              </div>
            </section>

            <section className="glass-card col-span-6" style={{ padding: "1.5rem" }}>
              <h3 className="section-title"><span>⛓️ Mouillage</span></h3>
              <div style={{ display: "flex", flexDirection: "column", gap: "1.25rem" }}>
                <div style={{ display: "grid", gridTemplateColumns: "1fr 1.5fr", gap: "1rem" }}>
                  <div>
                    <label className="label-modern">DN (mm)</label>
                    <select className="premium-input" value={selectedDN} onChange={(e) => setSelectedDN(Number(e.target.value))}>
                      <option value={0}>DN</option>
                      {availableDNs.map(dn => <option key={dn} value={dn}>{dn}</option>)}
                    </select>
                  </div>
                  <div>
                    <label className="label-modern">Type d'acier</label>
                    <select className="premium-input" value={selectedType} onChange={(e) => setSelectedType(e.target.value)}>
                      <option value="">Type</option>
                      {availableTypes.map(t => <option key={t} value={t}>{t}</option>)}
                    </select>
                  </div>
                </div>
                <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
                   <div>
                    <label className="label-modern">Lests</label>
                    <select className="premium-input" value={equipment.num_ballast} onChange={(e) => setEquipment({ ...equipment, num_ballast: Number(e.target.value) })}>
                      {availableBallasts.map(n => <option key={n} value={n}>{n} lests</option>)}
                    </select>
                  </div>
                  <div>
                    <label className="label-modern">Acier</label>
                    <select className="premium-input" value={chainQuality} onChange={(e) => setChainQuality(Number(e.target.value))}>
                      {[1, 2, 3].map(q => <option key={q} value={q}>Qualité Q{q}</option>)}
                    </select>
                  </div>
                </div>
                <div>
                  <label className="label-modern">Corps Mort (t/m³)</label>
                  <select className="premium-input" value={anchorDensity} onChange={(e) => setAnchorDensity(Number(e.target.value))}>
                    <option value={2.4}>Béton (2.4)</option>
                    <option value={7.85}>Acier (7.85)</option>
                  </select>
                </div>
              </div>
            </section>
          </div>

          <button 
            className="btn btn-primary" 
            style={{ width: "100%", height: "64px", fontSize: "1.1rem", borderRadius: "16px", letterSpacing: "0.1em" }}
            onClick={handleSimulate}
            disabled={loading || !selectedBuoy || !selectedDN}
          >
            {loading ? "CALCUL..." : "LANCER LA SIMULATION IALA"}
          </button>
        </div>

        {/* RIGHT COLUMN: PREVIEW & EQUIP SUMMARY */}
        <div className="col-span-4" style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
          
          {/* PREVIEW AT TOP */}
          <section className="glass-card" style={{ padding: "1.5rem", minHeight: "350px", display: "flex", flexDirection: "column" }}>
            <h3 className="section-title"><span>👁️ Aperçu 3D</span></h3>
            <div style={{ flex: 1, position: "relative", overflow: "hidden", borderRadius: "var(--radius-md)", background: "rgba(0,0,0,0.02)" }}>
              {selectedBuoyData ? (
                <BuoyRenderer buoy={selectedBuoyData} waterLine={0} />
              ) : (
                <div style={{ height: "100%", display: "flex", alignItems: "center", justifyContent: "center", color: "var(--text-muted)", fontSize: "0.9rem" }}>
                  Sélectionnez un modèle
                </div>
              )}
            </div>
            {selectedBuoyData && <div style={{ marginTop: "1rem", textAlign: "right", fontSize: "0.75rem", color: "var(--text-muted)" }}>{selectedBuoyData.name}</div>}
          </section>

          {/* EQUIPMENTS MODAL TRIGGER */}
          <section className="glass-card" style={{ padding: "1.5rem" }}>
            <h3 className="section-title"><span>📦 Équipements</span></h3>
            <div style={{ marginBottom: "1rem" }}>
              <div style={{ fontSize: "0.85rem", color: "var(--text-secondary)", marginBottom: "0.5rem" }}>
                {selectedEquipCount > 0 
                  ? `${selectedEquipCount} équipement(s) sélectionné(s)` 
                  : "Aucun équipement additionnel"}
              </div>
              <div style={{ fontSize: "1rem", fontWeight: 800, color: "var(--primary)" }}>
                + {totalEquipmentMass.toFixed(2)} kg
              </div>
            </div>
            <button 
              className="btn btn-secondary" 
              style={{ width: "100%", height: "48px" }}
              onClick={() => setIsEquipModalOpen(true)}
            >
              🛠️ Gérer les équipements
            </button>
          </section>
        </div>
      </div>

      {/* MODAL EQUIPMENTS */}
      {isEquipModalOpen && (
        <div style={{ 
          position: "fixed", inset: 0, zIndex: 1000, 
          background: "rgba(0,0,0,0.6)", backdropFilter: "blur(4px)",
          display: "flex", alignItems: "center", justifyContent: "center", padding: "2rem"
        }}>
          <div className="glass-card animate-scale-in" style={{ width: "100%", maxWidth: "800px", maxHeight: "80vh", display: "flex", flexDirection: "column", background: "var(--surface)", border: "1px solid var(--primary)" }}>
            <div style={{ padding: "1.5rem", borderBottom: "1px solid var(--border)", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <h2 style={{ fontSize: "1.2rem", fontWeight: 800, margin: 0 }}>Catalogue des Équipements</h2>
              <button 
                onClick={() => setIsEquipModalOpen(false)}
                style={{ background: "transparent", border: "none", fontSize: "1.5rem", cursor: "pointer", color: "var(--text-muted)" }}
              >&times;</button>
            </div>
            
            <div style={{ flex: 1, overflowY: "auto", padding: "1.5rem" }}>
              <EquipmentSelector 
                catalog={equipmentStandards}
                selected={selectedEquipments}
                onChange={setSelectedEquipments}
                totalMass={totalEquipmentMass}
              />
            </div>

            <div style={{ padding: "1.5rem", borderTop: "1px solid var(--border)", display: "flex", justifyContent: "flex-end" }}>
              <button className="btn btn-primary" onClick={() => setIsEquipModalOpen(false)} style={{ padding: "0.8rem 2rem" }}>
                Terminer
              </button>
            </div>
          </div>
        </div>
      )}

      {/* RESULTS AREA */}
      {result && (
        <div className="animate-fade-in" style={{ marginTop: "4rem" }}>
          <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "2rem" }}>
            <h2 style={{ fontSize: "1.75rem", fontWeight: 800, color: "var(--primary)" }}>RÉSULTATS</h2>
            <div style={{ 
              padding: "0.6rem 2rem", borderRadius: "var(--radius-full)", 
              background: result.is_safe ? "var(--success-bg)" : "var(--error-bg)",
              color: result.is_safe ? "#059669" : "#dc2626", 
              fontWeight: 800, border: `1px solid ${result.is_safe ? "#10b981" : "#f87171"}`
            }}>
              {result.is_safe ? "✓ CONFORME IALA" : "⚠ HORS LIMITES"}
            </div>
          </div>

          <div className="grid-cols-12">
            <ResultCard title="Flottabilité" span={4}>
                <ResultRow label="Enfoncement" value={result.enfoncement} unit="m" />
                <ResultRow label="Franc-Bord" value={result.franc_bord} unit="m" />
                <ResultRow label="Tirant d'Eau" value={result.tirant_eau} unit="m" />
                <ResultRow label="Masse Totale" value={result.masse_bouee_fixe} unit="kg" />
            </ResultCard>

            <ResultCard title="Efforts" span={4}>
                <ResultRow label="Tension Max" value={result.tension_max_mouillage} unit="t" highlight />
                <ResultRow label="Long. Caténaire" value={result.longueur_catenary} unit="m" />
                <ResultRow label="Effort Horizontal" value={result.effort_horizontal_kg} unit="kg" />
                <ResultRow label="Rayon Évitage" value={result.rayon_evitage} unit="m" />
            </ResultCard>

            <ResultCard title="Sécurité" span={4}>
                <ResultRow label="Coef. Chaine" value={result.coef_securite_chaine} unit="" highlight={result.coef_securite_chaine < 3} color={result.coef_securite_chaine < 3 ? "var(--color-error)" : "inherit"} />
                <ResultRow label="Masse Min C.M" value={result.masse_min_corps_mort} unit="kg" />
                <ResultRow label="Surf. Lat. Émergée" value={result.surface_laterale_emergee} unit="m²" />
                <ResultRow label="Réserve Flottab." value={result.reserve_flottabilite} unit="%" />
            </ResultCard>
          </div>
        </div>
      )}

      <style jsx>{`
        .label-modern { display: block; font-size: 0.7rem; font-weight: 600; color: var(--text-muted); margin-bottom: 0.4rem; text-transform: uppercase; letter-spacing: 0.05em; }
        @keyframes scaleIn { from { transform: scale(0.95); opacity: 0; } to { transform: scale(1); opacity: 1; } }
        .animate-scale-in { animation: scaleIn 0.2s cubic-bezier(0.16, 1, 0.3, 1); }
      `}</style>
    </div>
  );
}

const miniBtnStyle = (active: boolean) => ({
  flex: 1, padding: "0.25rem", fontSize: "0.65rem", 
  background: active ? "var(--primary)" : "var(--surface-muted)",
  color: active ? "white" : "inherit",
  borderRadius: "4px", border: "1px solid var(--border)", cursor: "pointer",
  transition: "0.2s"
});

function ResultCard({ title, children, span }: any) {
    return (
        <div className={`col-span-${span} glass-card`} style={{ padding: "1.25rem" }}>
            <h4 style={{ fontSize: "0.8rem", fontWeight: 700, color: "var(--text-muted)", marginBottom: "1rem", textTransform: "uppercase" }}>{title}</h4>
            <div style={{ display: "flex", flexDirection: "column", gap: "0.6rem" }}>{children}</div>
        </div>
    );
}

function ResultRow({ label, value, unit, highlight = false, color = "inherit" }: any) {
  return (
    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", paddingBottom: "0.5rem", borderBottom: "1px dotted var(--border)", color: color }}>
      <span style={{ fontSize: "0.8rem", fontWeight: 500, color: "var(--text-secondary)" }}>{label}</span>
      <span style={{ fontSize: highlight ? "1.1rem" : "0.95rem", fontWeight: 800, color: highlight ? "var(--primary)" : "inherit" }}>
        {typeof value === "number" ? value.toFixed(2) : value} 
        <span style={{ fontSize: "0.7rem", fontWeight: 500, marginLeft: "0.2rem", color: "var(--text-muted)" }}>{unit}</span>
      </span>
    </div>
  );
}
