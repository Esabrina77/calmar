"use client";

import { useSimulation } from "../hooks/useSimulation";
import BuoyRenderer from "./BuoyRenderer";

export default function SimulationDashboard() {
  const {
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
  } = useSimulation();

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "2rem" }}>
      {/* 1. SELECTION VISUELLE DE LA BOUÉE */}
      <section>
        <h3
          style={{
            fontSize: "0.8rem",
            fontWeight: 700,
            color: "var(--primary)",
            letterSpacing: "0.1em",
            marginBottom: "1rem",
          }}
        >
          CHOIX DU MODÈLE MOBILIS
        </h3>
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(150px, 1fr))",
            gap: "1rem",
          }}
        >
          {buoys.map((b) => (
            <div
              key={b.id}
              onClick={() => setSelectedBuoy(b.id)}
              style={{
                padding: "1.5rem",
                borderRadius: "var(--radius-md)",
                background:
                  selectedBuoy === b.id
                    ? "var(--primary-light)"
                    : "var(--surface-muted)",
                border: `2px solid ${selectedBuoy === b.id ? "var(--primary)" : "var(--border)"}`,
                cursor: "pointer",
                textAlign: "center",
                transition: "all 0.2s ease",
              }}
            >
              <div
                style={{
                  fontSize: "1.2rem",
                  fontWeight: 700,
                  marginBottom: "0.5rem",
                }}
              >
                {b.name}
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* 2. FORMULAIRE & VISUEL */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "1fr 300px",
          gap: "2rem",
          alignItems: "start",
        }}
      >
        {/* COLONNE GAUCHE : FORMULAIRES */}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))",
            gap: "2rem",
          }}
        >
          {/* PARAMS SITE */}
          <div
            className="card"
            style={{
              padding: "2rem",
              background: "var(--surface)",
              borderRadius: "var(--radius-lg)",
              border: "1px solid var(--border)",
            }}
          >
            <h4 style={{ marginBottom: "1.5rem", fontSize: "1rem" }}>
              📍 ENVIRONNEMENT
            </h4>

            <div style={{ display: "grid", gap: "1rem" }}>
              <label style={labelStyle}>
                PROF. D'EAU (m)
                <input
                  type="number"
                  value={site.water_depth}
                  onChange={(e) =>
                    setSite({ ...site, water_depth: Number(e.target.value) })
                  }
                  style={inputStyle}
                />
              </label>
              <label style={labelStyle}>
                MARNAGE (m)
                <input
                  type="number"
                  value={site.marnage}
                  onChange={(e) =>
                    setSite({ ...site, marnage: Number(e.target.value) })
                  }
                  style={inputStyle}
                />
              </label>
              <div
                style={{
                  display: "grid",
                  gridTemplateColumns: "1fr 1fr",
                  gap: "1rem",
                }}
              >
                <label style={labelStyle}>
                  HOULE MAX (m)
                  <input
                    type="number"
                    value={site.wave_height}
                    onChange={(e) => updateWaveHeight(Number(e.target.value))}
                    style={inputStyle}
                  />
                </label>
                <label style={labelStyle}>
                  SIGNIFICATIVE (m)
                  <input
                    type="number"
                    value={site.wave_significant}
                    readOnly
                    style={{
                      ...inputStyle,
                      background: "var(--surface-muted)",
                      cursor: "not-allowed",
                    }}
                  />
                </label>
              </div>
              <label style={labelStyle}>
                VENT MAX (m/s)
                <input
                  type="number"
                  value={site.wind_velocity}
                  onChange={(e) =>
                    setSite({ ...site, wind_velocity: Number(e.target.value) })
                  }
                  style={inputStyle}
                />
              </label>
              <label style={labelStyle}>
                COURANT (m/s)
                <input
                  type="number"
                  value={site.current_velocity}
                  step="0.1"
                  onChange={(e) =>
                    setSite({
                      ...site,
                      current_velocity: Number(e.target.value),
                    })
                  }
                  style={inputStyle}
                />
              </label>
              <div
                style={{ display: "flex", gap: "1rem", marginTop: "0.5rem" }}
              >
                <button
                  onClick={() => setSite({ ...site, water_density: 1025 })}
                  style={{
                    ...tabStyle,
                    background:
                      site.water_density === 1025
                        ? "var(--primary)"
                        : "var(--surface-muted)",
                  }}
                >
                  Eau Salée
                </button>
                <button
                  onClick={() => setSite({ ...site, water_density: 1000 })}
                  style={{
                    ...tabStyle,
                    background:
                      site.water_density === 1000
                        ? "var(--primary)"
                        : "var(--surface-muted)",
                  }}
                >
                  Eau Douce
                </button>
              </div>
            </div>
          </div>

          {/* PARAMS MOUILLAGE */}
          <div
            className="card"
            style={{
              padding: "2rem",
              background: "var(--surface)",
              borderRadius: "var(--radius-lg)",
              border: "1px solid var(--border)",
            }}
          >
            <h4 style={{ marginBottom: "1.5rem", fontSize: "1rem" }}>
              ⛓️ LIGNE DE MOUILLAGE
            </h4>
            <div style={{ display: "grid", gap: "1rem" }}>
              <label style={labelStyle}>
                CHAÎNE
                <select
                  value={selectedChain}
                  onChange={(e) => setSelectedChain(Number(e.target.value))}
                  style={inputStyle}
                >
                  {chains.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.type} (DN {c.id})
                    </option>
                  ))}
                </select>
              </label>
              <label style={labelStyle}>
                QUALITÉ ACIER
                <select
                  value={chainQuality}
                  onChange={(e) => setChainQuality(e.target.value)}
                  style={inputStyle}
                >
                  <option value="Q1">Grade Q1 (Standard)</option>
                  <option value="Q2">Grade Q2 (Haute résistance)</option>
                  <option value="Q3">Grade Q3 (Extra haute résistance)</option>
                </select>
              </label>
              <label style={labelStyle}>
                LONGUEUR (m)
                <input
                  type="number"
                  value={mooring.chain_length}
                  onChange={(e) =>
                    setMooring({
                      ...mooring,
                      chain_length: Number(e.target.value),
                    })
                  }
                  style={inputStyle}
                />
              </label>
              <label style={labelStyle}>
                NB DE GUEUSES (Lest)
                <input
                  type="number"
                  value={equipment.num_ballast}
                  onChange={(e) =>
                    setEquipment({
                      ...equipment,
                      num_ballast: Number(e.target.value),
                    })
                  }
                  style={inputStyle}
                />
              </label>
              <label style={labelStyle}>
                MASSE ÉQUIPEMENT (kg)
                <input
                  type="number"
                  value={equipment.mass_equipment}
                  onChange={(e) =>
                    setEquipment({
                      ...equipment,
                      mass_equipment: Number(e.target.value),
                    })
                  }
                  style={inputStyle}
                />
              </label>
              <label style={labelStyle}>
                TYPE DE CORPS MORT
                <select
                  value={anchorType}
                  onChange={(e) => setAnchorType(e.target.value)}
                  style={inputStyle}
                >
                  <option value="Béton">Béton (2.4 t/m³)</option>
                  <option value="Acier">Acier (7.8 t/m³)</option>
                </select>
              </label>
            </div>
          </div>
        </div>

        {/* COLONNE DROITE : LE VISUEL MOBILIS */}
        <div
          className="card"
          style={{
            position: "sticky",
            top: "100px",
            padding: "2rem",
            background: "var(--surface)",
            borderRadius: "var(--radius-lg)",
            border: "2px solid var(--primary)",
            display: "flex",
            justifyContent: "center",
            minHeight: "400px",
            alignItems: "center",
          }}
        >
          {selectedBuoyData ? (
            <BuoyRenderer buoy={selectedBuoyData} width={250} height={400} />
          ) : (
            <div style={{ textAlign: "center", color: "var(--text-muted)" }}>
              <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>⚓</div>
              <p>Sélectionnez un modèle pour voir le visuel Mobilis</p>
            </div>
          )}
        </div>
      </div>

      <div style={{ display: "flex", justifyContent: "center" }}>
        <button
          onClick={handleSimulate}
          disabled={loading || !selectedBuoy}
          className="btn btn-primary"
          style={{ padding: "1.2rem 4rem", fontSize: "1.1rem" }}
        >
          {loading ? "CALCUL EN COURS..." : "LANCER LA SIMULATION"}
        </button>
      </div>

      {/* 3. RÉSULTATS IALA */}
      {result && (
        <section
          className="animate-fade-in"
          style={{
            background: "var(--surface-muted)",
            padding: "2.5rem",
            borderRadius: "var(--radius-lg)",
            border: "2px solid var(--primary)",
            boxShadow: "0 20px 40px rgba(0,0,0,0.2)",
          }}
        >
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              marginBottom: "2rem",
            }}
          >
            <h2 style={{ fontSize: "1.5rem", fontWeight: 800 }}>
              RAPPORTS DE CALCUL IALA
            </h2>
            <div
              style={{
                padding: "0.5rem 1rem",
                background: result.IsSafe
                  ? "var(--success-bg)"
                  : "var(--error-bg)",
                color: result.IsSafe
                  ? "var(--color-success)"
                  : "var(--color-error)",
                borderRadius: "var(--radius-full)",
                fontWeight: 700,
              }}
            >
              {result.IsSafe ? "VALIDE" : "CRITIQUE"}
            </div>
          </div>

          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
              gap: "2rem",
            }}
          >
            <ResultCard
              label="ENFONCEMENT"
              value={result.enfoncement}
              unit="m"
            />
            <ResultCard
              label="TENSION MAX"
              value={result.tension_max_mouillage}
              unit="t"
              color={
                result.tension_max_mouillage > 1
                  ? "var(--color-error)"
                  : "inherit"
              }
            />
            <ResultCard label="FRANC-BORD" value={result.franc_bord} unit="m" />
            <ResultCard label="ÉVITAGE" value={result.rayon_evitage} unit="m" />
            <ResultCard
              label="COEF. SÉCURITÉ"
              value={result.coef_securite_chaine}
              unit=""
            />
            <ResultCard
              label="CORPS MORT MIN."
              value={result.masse_min_corps_mort}
              unit="kg"
            />
          </div>
        </section>
      )}
    </div>
  );
}

function ResultCard({ label, value, unit, color }: any) {
  return (
    <div>
      <div
        style={{
          fontSize: "0.7rem",
          color: "var(--text-muted)",
          fontWeight: 700,
          letterSpacing: "0.1em",
        }}
      >
        {label}
      </div>
      <div
        style={{
          fontSize: "2rem",
          fontWeight: 800,
          color: color || "var(--text-primary)",
          marginTop: "0.5rem",
        }}
      >
        {typeof value === "number" ? value.toFixed(2) : value}{" "}
        <span style={{ fontSize: "0.9rem", color: "var(--text-muted)" }}>
          {unit}
        </span>
      </div>
    </div>
  );
}

const labelStyle: any = {
  display: "block",
  fontSize: "0.75rem",
  fontWeight: 600,
  color: "var(--text-muted)",
  marginBottom: "0.2rem",
};

const inputStyle: any = {
  width: "100%",
  padding: "0.7rem",
  background: "var(--input-bg)",
  border: "1px solid var(--input-border)",
  borderRadius: "var(--radius-sm)",
  color: "var(--text-primary)",
  fontSize: "0.9rem",
};

const tabStyle: any = {
  flex: 1,
  padding: "0.6rem",
  border: "none",
  borderRadius: "var(--radius-sm)",
  color: "white",
  fontSize: "0.8rem",
  cursor: "pointer",
  transition: "all 0.2s",
};
