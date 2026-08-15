package consumer

import (
	"context"
	"strings"
	"testing"

	"notification-service/internal/mail"
)

type fakeMailer struct{ sent []mail.Mail }

func (f *fakeMailer) Send(_ context.Context, m mail.Mail) error {
	f.sent = append(f.sent, m)
	return nil
}

// Exact System.Text.Json output of the .NET PurchaseEvent.
const paymentJSON = `{
  "UserEmail": "buyer@example.com",
  "Cart": {
    "Id": "11111111-1111-1111-1111-111111111111",
    "Items": [
      {
        "Id": "22222222-2222-2222-2222-222222222222",
        "Product": {
          "Id": "22222222-2222-2222-2222-222222222222",
          "Name": "Dune",
          "Price": 30,
          "ImageUrl": "http://img/dune.png",
          "CategoryName": "Sci-Fi",
          "IsOnSale": true,
          "SalePercent": 20,
          "EffectivePrice": 24
        },
        "Amount": 2
      }
    ]
  },
  "AuthorNotices": [
    {
      "AuthorEmail": "herbert@example.com",
      "AuthorName": "Frank Herbert",
      "ProductId": "22222222-2222-2222-2222-222222222222",
      "ProductName": "Dune",
      "Amount": 2,
      "UnitPricePaid": 24,
      "SalePercentOff": 20
    }
  ]
}`

const promoJSON = `{
  "RecipientEmails": ["fan@example.com"],
  "ProductId": "33333333-3333-3333-3333-333333333333",
  "ProductName": "Dune",
  "OriginalPrice": 30,
  "DiscountedPrice": 24,
  "PercentOff": 20,
  "CategoryName": "Sci-Fi"
}`

func TestDispatchPayment(t *testing.T) {
	recipient := &fakeMailer{}
	handler := NewDispatchHandler(recipient)

	err := handler(context.Background(), "payment-completed", []byte(paymentJSON))
	if err != nil {
		t.Fatal(err)
	}

	if len(recipient.sent) != 2 {
		t.Fatalf("sent %d mails, want 2 (receipt + author notice)", len(recipient.sent))
	}
	if recipient.sent[0].To != "buyer@example.com" || !strings.Contains(recipient.sent[0].Subject, "receipt") {
		t.Errorf("receipt mail wrong: %+v", recipient.sent[0])
	}
	if recipient.sent[1].To != "herbert@example.com" || !strings.Contains(recipient.sent[1].Subject, "sale") {
		t.Errorf("author notice mail wrong: %+v", recipient.sent[1])
	}
}

func TestDispatchPromotion(t *testing.T) {
	recipient := &fakeMailer{}
	handler := NewDispatchHandler(recipient)

	err := handler(context.Background(), "promotion-created", []byte(promoJSON))
	if err != nil {
		t.Fatal(err)
	}

	if len(recipient.sent) != 1 || recipient.sent[0].To != "fan@example.com" {
		t.Fatalf("promo mails wrong: %+v", recipient.sent)
	}
}

func TestDispatchRejectsMalformed(t *testing.T) {
	handler := NewDispatchHandler(&fakeMailer{})
	err := handler(context.Background(), "payment-completed", []byte(`{not json`))
	if err == nil {
		t.Fatal("expected error for malformed json")
	}
}

func TestDispatchIgnoresUnknownTopic(t *testing.T) {
	recipient := &fakeMailer{}
	handler := NewDispatchHandler(recipient)
	err := handler(context.Background(), "unknown-topic", []byte("{}"))
	if err != nil {
		t.Fatal(err)
	}
	if len(recipient.sent) != 0 {
		t.Fatal("unknown topic must not send mails")
	}
}
