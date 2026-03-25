package api_test

import (
	"bytes"
	"encoding/json"
	"io"
	"net/http"
	"testing"

	"backend/internal/api"
	"backend/internal/db"
	"backend/internal/models"

	"github.com/joho/godotenv"
	"github.com/stretchr/testify/assert"
)

func TestSimulationRouteIntegrity(t *testing.T) {
	// 1. Setup
	if err := godotenv.Load("../../.env"); err != nil {
		t.Log("ℹ️ .env non trouvé")
	}
	db.InitDatabase()
	app := api.SetupApp()

	// 2. Préparer une requête réaliste
	// Il nous faut un ID de bouée et de chaîne valide
	var buoy models.Buoy
	db.DB.First(&buoy)
	var chain models.Chain
	db.DB.First(&chain)

	if buoy.ID == 0 || chain.ID == 0 {
		t.Skip("⚠️ Base de données vide, impossible de tester la simulation")
	}

	payload := map[string]interface{}{
		"buoy_id":    buoy.ID,
		"chain_id":   chain.ID,
		"num_ballast": 1,
		"conditions": map[string]float64{
			"WaterDepth":      10,
			"WindVelocity":    10,
			"CurrentVelocity": 0.5,
		},
	}
	body, _ := json.Marshal(payload)

	req, _ := http.NewRequest("POST", "/api/simulate", bytes.NewBuffer(body))
	req.Header.Set("Content-Type", "application/json")

	// 3. Exécuter
	resp, err := app.Test(req)

	// 4. Assertions
	assert.Nil(t, err)
	assert.Equal(t, 200, resp.StatusCode)

	respBody, _ := io.ReadAll(resp.Body)
	assert.Contains(t, string(respBody), "enfoncement")
	t.Logf("✅ Réponse Simulation : %s", string(respBody))
}
