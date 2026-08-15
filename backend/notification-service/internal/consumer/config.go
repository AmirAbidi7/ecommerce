package consumer

import "os"

type Config struct {
	Bootstrap string
	Group     string
	Topics    []string
}

func LoadConfig() Config {
	bootstrap := os.Getenv("KAFKA_BOOTSTRAP")
	if bootstrap == "" {
		bootstrap = "localhost:9092"
	}
	group := os.Getenv("KAFKA_GROUP_ID")
	if group == "" {
		group = "mailing-service"
	}
	return Config{
		Bootstrap: bootstrap,
		Group:     group,
		Topics:    []string{"payment-completed", "promotion-created"},
	}
}
