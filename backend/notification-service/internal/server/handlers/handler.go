package handlers

import (
	"notification-service/internal/database"

	"github.com/gofiber/fiber/v3"
	"gorm.io/gorm"
)

type Handler struct {
	DB *gorm.DB
}

func New() (*Handler, error) {
	db, err := database.GetDB()
	if err != nil {
		return nil, err
	}
	return &Handler{DB: db}, nil
}

func (h *Handler) SetupRoutes(r fiber.Router) {
	r.Get("/", h.HelloWorldHandler)
}
