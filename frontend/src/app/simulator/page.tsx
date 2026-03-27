import Navbar from "@/components/Navbar";
import SimulationDashboard from "@/components/SimulationDashboard";

export default function SimulatorPage() {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        minHeight: "100vh",
        background: "var(--background)",
      }}
    >
      <Navbar />

      <main className="container" style={{ flex: 1, padding: "40px 0" }}>
        <header style={{ marginBottom: "40px" }}>
          <div style={{ display: "flex", alignItems: "baseline", gap: "1rem" }}>
            <h1 style={{ fontSize: "2.5rem", fontWeight: 800, marginBottom: "0.5rem" }}>
              SIMULATEUR DE MOUILLAGE
            </h1>
            <span style={{ fontSize: "0.9rem", color: "var(--primary)", fontWeight: 700, opacity: 0.7 }}>
              Calmar Web v2
            </span>
          </div>
          <p style={{ color: "var(--text-muted)", fontSize: "1.1rem" }}>
            Outil de calcul caténaire statique. <span style={{ fontSize: "0.9rem", opacity: 0.6 }}>| Simulation IALA Premium</span>
          </p>
        </header>

        <SimulationDashboard />

        {/* FOOTER INFO */}
        <div
          style={{
            marginTop: "50px",
            padding: "1.5rem",
            background: "var(--surface-muted)",
            borderRadius: "var(--radius-md)",
            border: "1px solid var(--border)",
            fontSize: "0.85rem",
          }}
        >
          <p style={{ color: "var(--text-muted)" }}>
            <strong>Note technique :</strong> En cas de résultats incohérents,
            veuillez vérifier vos données d'entrée ou contacter le service
            technique.
          </p>
        </div>
      </main>
    </div>
  );
}
