from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI(
    title="Calmar API",
    description="API de calcul de lignes d'amarrage caténaire (IALA 1066)",
    version="1.0.0"
)

# Configuration CORS pour autoriser le Frontend (Next.js)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],  # URL du frontend Next.js en dev
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/")
def read_root():
    return {
        "status": "online",
        "message": "Bienvenue sur l'API Calmar. Prête pour les calculs d'amarrage.",
        "version": "1.0.0"
    }

@app.get("/health")
def health_check():
    return {"status": "ok"}
