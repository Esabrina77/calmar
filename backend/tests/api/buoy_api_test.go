package api_test

import (
	"backend/internal/api"
	"backend/internal/db"
	"io"
	"net/http"
	"testing"

	"github.com/joho/godotenv"
	"github.com/stretchr/testify/assert"
)

// TestGetBuoysVerifyRoute valide que l'accès à la liste des bouées fonctionne
func TestGetBuoysRoute(t *testing.T) {
	// 1. Setup
	if err := godotenv.Load("../../.env"); err != nil {
		t.Log("ℹ️ .env non trouvé, continuation avec variables d'environnement")
	}
	db.InitDatabase()
	app := api.SetupApp()

	// 2. Création de la requête Mock
	req, _ := http.NewRequest("GET", "/api/buoys", nil)

	// 3. Exécution du test via Fiber
	resp, err := app.Test(req)

	// 4. Assertions
	assert.Nil(t, err)
	assert.Equal(t, 200, resp.StatusCode)

	// Vérifier que le body n'est pas vide
	body, _ := io.ReadAll(resp.Body)
	assert.NotEmpty(t, body)
	t.Logf("✅ Réponse /api/buoys : %s", string(body))
}

// TestGetBuoyDetail valide la route de détail
func TestGetBuoyDetailRoute(t *testing.T) {
	db.InitDatabase()
	app := api.SetupApp()

	// On tente d'accéder à l'ID 1 (devrait exister après le parser)
	req, _ := http.NewRequest("GET", "/api/buoys/1", nil)
	resp, _ := app.Test(req)

	assert.True(t, resp.StatusCode == 200 || resp.StatusCode == 404)
}
