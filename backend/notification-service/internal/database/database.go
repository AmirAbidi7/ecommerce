package database

import (
	"errors"
	"os"

	"github.com/joho/godotenv"
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

func GetDB() (*gorm.DB, error) {
	err := godotenv.Load(".env")
	if err != nil {
		return nil, err
	}

	dsn, exists := os.LookupEnv("dsn")
	if !exists {
		return nil, errors.New("variable not found")
	}

	db, err := gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		return nil, err
	}

	return db, nil
}
