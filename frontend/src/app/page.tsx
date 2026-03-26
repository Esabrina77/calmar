import Link from "next/link";
import Navbar from "@/components/Navbar";

export default function Home() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', background: 'var(--background)' }}>
      <Navbar />
      
      <main style={{ flex: 1 }}>
        {/* HERO SECTION */}
        <section style={{ padding: '100px 0', position: 'relative', overflow: 'hidden' }}>
          {/* Glow Effect */}
          <div style={{ 
            position: 'absolute', 
            top: 0, 
            right: 0, 
            zIndex: -1, 
            width: '600px', 
            height: '600px', 
            background: 'var(--primary-glow)', 
            filter: 'blur(130px)', 
            borderRadius: '50%',
            opacity: 0.2,
            transform: 'translate(40%, -40%)'
          }}></div>

          <div className="container" style={{ textAlign: 'left' }}>
            <div className="animate-fade-in" style={{ maxWidth: '900px' }}>
              <h1 style={{ 
                fontSize: 'clamp(2.5rem, 6vw, 5.5rem)', 
                lineHeight: 1, 
                fontWeight: 800, 
                marginBottom: '2rem',
                letterSpacing: '-0.03em'
              }}>
                MOTEUR PHYSIQUE <br />
                <span style={{ color: 'var(--primary)', letterSpacing: '0.1em' }}>IALA CERTIFIÉ</span>
              </h1>
              <p style={{ 
                fontSize: '1.25rem', 
                color: 'var(--text-secondary)', 
                marginBottom: '3rem', 
                maxWidth: '650px',
                lineHeight: 1.6
              }}>
                Précision chirurgicale pour vos lignes de mouillage caténaires. 
                Configurez vos sites, simulez les tempêtes et sécurisez vos déploiements Mobilis.
              </p>
              
              <div style={{ display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
                <Link href="/simulator">
                  <button className="btn btn-primary" style={{ fontSize: '1rem', padding: '1.2rem 2.5rem' }}>
                    LANCER UNE SIMULATION
                  </button>
                </Link>
                <Link href="/buoys">
                  <button className="btn btn-secondary" style={{ fontSize: '1rem', padding: '1.2rem 2.5rem' }}>
                    CONSULTER LE CATALOGUE
                  </button>
                </Link>
              </div>
            </div>

            {/* SEPARATOR */}
            <div style={{ width: '100px', height: '4px', background: 'var(--primary)', marginTop: '80px', marginBottom: '40px' }}></div>

            {/* QUICK STATS */}
            <div style={{ 
              display: 'grid', 
              gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', 
              gap: '2rem',
              marginTop: '2rem'
            }}>
              <div style={{ padding: '2.5rem', background: 'var(--surface-muted)', border: '1px solid var(--border)', borderRadius: 'var(--radius-lg)' }}>
                <h3 style={{ color: 'var(--primary)', fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.2em', marginBottom: '1rem' }}>01. CONFIGURATION</h3>
                <p style={{ fontSize: '1rem', color: 'var(--text-muted)' }}>Sélectionnez vos modèles de bouées et types de chaînes Mobilis en un clic.</p>
              </div>
              <div style={{ padding: '2.5rem', background: 'var(--surface-muted)', border: '1px solid var(--border)', borderRadius: 'var(--radius-lg)' }}>
                <h3 style={{ color: 'var(--primary)', fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.2em', marginBottom: '1rem' }}>02. ENVIRONNEMENT</h3>
                <p style={{ fontSize: '1rem', color: 'var(--text-muted)' }}>Intégrez vent, courant et houle. Support de l'eau douce et salée.</p>
              </div>
              <div style={{ padding: '2.5rem', background: 'var(--surface-muted)', border: '1px solid var(--border)', borderRadius: 'var(--radius-lg)' }}>
                <h3 style={{ color: 'var(--primary)', fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.2em', marginBottom: '1rem' }}>03. CERTIFICATION</h3>
                <p style={{ fontSize: '1rem', color: 'var(--text-muted)' }}>Résultats validés selon les standards IALA E-108 pour une sécurité maximale.</p>
              </div>
            </div>
          </div>
        </section>
      </main>

      <footer style={{ borderTop: '1px solid var(--border)', padding: '50px 0', background: 'var(--surface)', color: 'var(--text-muted)', fontSize: '0.9rem' }}>
        <div className="container" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
          <p>© 2026 MOBILIS SA - CALMAR WEB ENGINE</p>
          <div style={{ display: 'flex', gap: '2rem' }}>
            <a href="#">Support</a>
            <a href="#">Standards IALA</a>
            <a href="#">Contact</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
