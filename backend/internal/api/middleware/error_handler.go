package middleware

import (
	"github.com/gofiber/fiber/v2"
	"log"
)

// ErrorHandler capture toutes les erreurs Go/Fiber et renvoie un JSON propre au Frontend
func ErrorHandler(c *fiber.Ctx, err error) error {
	// Debug log
	log.Printf("🚨 API ERROR: %v", err)

	// Statut par défaut 500
	code := fiber.StatusInternalServerError

	// Si c'est une erreur Fiber connue (ex: 404, 400)
	if e, ok := err.(*fiber.Error); ok {
		code = e.Code
	}

	return c.Status(code).JSON(fiber.Map{
		"success": false,
		"message": err.Error(),
	})
}
