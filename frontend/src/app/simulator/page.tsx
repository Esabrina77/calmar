import Navbar from "@/components/Navbar";
import SimulationDashboard from "@/components/SimulationDashboard";

export default function SimulatorPage() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', background: 'var(--background)' }}>
      <Navbar />
      
      <main className="container" style={{ flex: 1, padding: '40px 0' }}>
        <header style={{ marginBottom: '40px' }}>
          <h1 style={{ fontSize: '2.5rem', fontWeight: 800, marginBottom: '0.5rem' }}>SIMULATEUR DE MOUILLAGE</h1>
          <p style={{ color: 'var(--text-muted)' }}>Outil de calcul caténaire statique certifié Mobilis SA.</p>
        </header>

        <SimulationDashboard />
        
        {/* FOOTER INFO */}
        <div style={{ marginTop: '50px', padding: '1.5rem', background: 'var(--surface-muted)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border)', fontSize: '0.85rem' }}>
          <p style={{ color: 'var(--text-muted)' }}>
            <strong>Note technique :</strong> Les calculs sont basés sur les abaques de l'IALA. En cas de résultats incohérents, veuillez vérifier vos données d'entrée ou contacter Mobilis.
          </p>
        </div>
      </main>
    </div>
  );
}
