package handlers

import "github.com/gofiber/fiber/v3"

func (h *Handler) HelloWorldHandler(c fiber.Ctx) error {
	resp := fiber.Map{
		"message": "Hello World",
	}

	return c.JSON(resp)
}
