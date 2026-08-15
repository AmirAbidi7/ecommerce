package mail

import (
	"context"
	"encoding/json"
	"log/slog"
)

type Mail struct {
	To      string
	Subject string
	Body    string
}

type Mailer interface {
	Send(ctx context.Context, m Mail) error
}

// LogMailer writes each mail as a JSON log line. Swap for an SMTP mailer later.
type LogMailer struct{ logger *slog.Logger }

func NewLogMailer(logger *slog.Logger) *LogMailer { return &LogMailer{logger: logger} }

func (m *LogMailer) Send(_ context.Context, mail Mail) error {
	bytes, err := json.Marshal(map[string]string{
		"to":      mail.To,
		"subject": mail.Subject,
		"body":    mail.Body,
	})
	if err != nil {
		return err
	}
	m.logger.Info("mail", "payload", string(bytes))
	return nil
}
