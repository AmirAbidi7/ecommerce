package app

import (
	"notification-service/internal/server/handlers"

	"github.com/gofiber/fiber/v3"
	"github.com/gofiber/fiber/v3/middleware/cors"
)

type FiberServer struct {
	*fiber.App
}

func New() *FiberServer {
	server := &FiberServer{
		App: fiber.New(fiber.Config{
			ServerHeader: "notification-service",
			AppName:      "notification-service",
		}),
	}

	return server
}

func (s *FiberServer) RegisterFiberRoutes(h *handlers.Handler) {
	s.App.Use(cors.New(cors.Config{
		AllowOrigins:     []string{"*"},
		AllowMethods:     []string{"GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH"},
		AllowHeaders:     []string{"Accept", "Authorization", "Content-Type"},
		AllowCredentials: false,
		MaxAge:           300,
	}))

	api := s.App.Group("/api")
	h.SetupRoutes(api)
}
