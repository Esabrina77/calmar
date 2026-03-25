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

func TestGetChainsRoute(t *testing.T) {
	if err := godotenv.Load("../../.env"); err != nil {
		t.Log("ℹ️ .env non trouvé")
	}
	db.InitDatabase()
	app := api.SetupApp()

	req, _ := http.NewRequest("GET", "/api/chains", nil)
	resp, err := app.Test(req)

	assert.Nil(t, err)
	assert.Equal(t, 200, resp.StatusCode)

	body, _ := io.ReadAll(resp.Body)
	assert.NotEmpty(t, body)
}
