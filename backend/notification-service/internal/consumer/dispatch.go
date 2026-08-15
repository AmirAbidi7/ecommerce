package consumer

import (
	"context"
	"encoding/json"
	"fmt"

	"notification-service/internal/mail"
)

const (
	topicPaymentCompleted = "payment-completed"
	topicPromotionCreated = "promotion-created"
)

func NewDispatchHandler(mailer mail.Mailer) func(ctx context.Context, topic string, value []byte) error {
	return func(ctx context.Context, topic string, value []byte) error {
		switch topic {
		case topicPaymentCompleted:
			var e mail.PurchaseEvent
			if err := json.Unmarshal(value, &e); err != nil {
				return fmt.Errorf("parse payment event: %w", err)
			}
			if err := mailer.Send(ctx, mail.BuildReceipt(e)); err != nil {
				return err
			}
			for _, notice := range e.AuthorNotices {
				if err := mailer.Send(ctx, mail.BuildAuthorNotice(notice, e.UserEmail)); err != nil {
					return err
				}
			}
			return nil

		case topicPromotionCreated:
			var e mail.PromotionEvent
			if err := json.Unmarshal(value, &e); err != nil {
				return fmt.Errorf("parse promotion event: %w", err)
			}
			for _, m := range mail.BuildPromoMails(e) {
				if err := mailer.Send(ctx, m); err != nil {
					return err
				}
			}
			return nil

		default:
			return nil
		}
	}
}
