package tests

import (
	"math"
	"testing"
	"backend/internal/calc"
)

// TestPhysicsIALA valide les formules mathématiques isolées.
// Ces tests ne dépendent pas de la base de données.
func TestPhysicsIALA(t *testing.T) {
	
	t.Run("ACosh", func(t *testing.T) {
		got := calc.ACosh(1.5)
		expected := 0.9624236501192
		if math.Abs(got-expected) > 1e-10 {
			t.Errorf("ACosh(1.5) = %f; voulu %f", got, expected)
		}
	})

	t.Run("Archimede", func(t *testing.T) {
		// 1m3 d'eau salée (1025kg/m3) * G (9.81) = 10055.25 Newtons
		got := calc.CalculateBuoyancy(1.0)
		expected := 1025.0 * 9.81
		if got != expected {
			t.Errorf("Archimede(1m3) = %f; voulu %f", got, expected)
		}
	})

	t.Run("WindDrag", func(t *testing.T) {
		// Vent 20m/s, Surface 2m2, Cd 1.2
		// Force (N) = 0.5 * 1.225 * 400 * 1.2 * 2.0 = 588 N
		// Effort (kgf) = 588 / 9.81 = 59.9388...
		got := calc.CalculateWindDrag(20.0, 2.0)
		if math.Abs(got-59.9388) > 0.1 {
			t.Errorf("WindDrag(20m/s, 2m2) = %f; voulu ~59.94kgf", got)
		}
	})
}
