package main

import (
	"context"
	"errors"
	"fmt"
	"log"
	"log/slog"
	"net/http"
	"os"
	"os/signal"
	"strconv"
	"syscall"
	"time"

	"notification-service/internal/consumer"
	"notification-service/internal/mail"
	"notification-service/internal/server/app"
	"notification-service/internal/server/handlers"

	_ "github.com/joho/godotenv/autoload"
)

func gracefulShutdown(fiberServer *app.FiberServer, done chan bool) {
	// Create context that listens for the interrupt signal from the OS.
	ctx, stop := signal.NotifyContext(context.Background(), syscall.SIGINT, syscall.SIGTERM)
	defer stop()

	// Listen for the interrupt signal.
	<-ctx.Done()

	log.Println("shutting down gracefully, press Ctrl+C again to force")
	stop() // Allow Ctrl+C to force shutdown

	// The context is used to inform the server it has 5 seconds to finish
	// the request it is currently handling
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	if err := fiberServer.ShutdownWithContext(ctx); err != nil {
		log.Printf("Server forced to shutdown with error: %v", err)
	}

	log.Println("Server exiting")

	// Notify the main goroutine that the shutdown is complete
	done <- true
}

func newMailDispatch(logger *slog.Logger) func(ctx context.Context, topic string, value []byte) error {
	return consumer.NewDispatchHandler(mail.NewLogMailer(logger))
}

func main() {
	h, err := handlers.New()
	if err != nil {
		panic(err)
	}

	server := app.New()

	server.RegisterFiberRoutes(h)

	client, err := consumer.New(consumer.LoadConfig())
	if err != nil {
		panic(err)
	}
	logger := slog.New(slog.NewTextHandler(os.Stdout, nil))
	go func() {
		if err := consumer.Run(context.Background(), client, newMailDispatch(logger), logger); err != nil {
			logger.Error("consumer exited", "error", err)
		}
	}()

	// Create a done channel to signal when the shutdown is complete
	done := make(chan bool, 1)

	go func() {
		port, _ := strconv.Atoi(os.Getenv("PORT"))
		err := server.Listen(fmt.Sprintf(":%d", port))
		if err != nil && !errors.Is(err, http.ErrServerClosed) {
			panic(fmt.Sprintf("http server error: %s", err))
		}
	}()

	// Run graceful shutdown in a separate goroutine
	go gracefulShutdown(server, done)

	// Wait for the graceful shutdown to complete
	<-done
	log.Println("Graceful shutdown complete.")
}
