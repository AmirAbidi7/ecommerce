package mail

import (
	"testing"
)

func purchaseEvent() PurchaseEvent {
	return PurchaseEvent{
		UserEmail: "buyer@example.com",
		Cart: CartOverView{
			Id: "11111111-1111-1111-1111-111111111111",
			Items: []CartProduct{
				{
					Id: "22222222-2222-2222-2222-222222222222",
					Product: ProductOverview{
						Id:             "22222222-2222-2222-2222-222222222222",
						Name:           "Dune",
						Price:          30,
						ImageUrl:       "http://img/dune.png",
						CategoryName:   "Sci-Fi",
						IsOnSale:       true,
						SalePercent:    intPtr(20),
						EffectivePrice: 24,
					},
					Amount: 2,
				},
			},
		},
		AuthorNotices: []AuthorNotice{
			{
				AuthorEmail:    "herbert@example.com",
				AuthorName:     "Frank Herbert",
				ProductId:      "22222222-2222-2222-2222-222222222222",
				ProductName:    "Dune",
				Amount:         2,
				UnitPricePaid:  24,
				SalePercentOff: intPtr(20),
			},
		},
	}
}

func intPtr(i int) *int { return &i }

func TestBuildReceipt(t *testing.T) {
	m := BuildReceipt(purchaseEvent())
	if m.To != "buyer@example.com" {
		t.Errorf("To = %q, want buyer email", m.To)
	}
	if m.Subject != "Order receipt #11111111-1111-1111-1111-111111111111" {
		t.Errorf("Subject = %q", m.Subject)
	}
	for _, want := range []string{"Dune", "24", "48"} {
		if !contains(m.Body, want) {
			t.Errorf("receipt body missing %q:\n%s", want, m.Body)
		}
	}
}

func TestBuildAuthorNotice(t *testing.T) {
	e := purchaseEvent()
	m := BuildAuthorNotice(e.AuthorNotices[0], e.UserEmail)
	if m.To != "herbert@example.com" {
		t.Errorf("To = %q", m.To)
	}
	for _, want := range []string{"Frank Herbert", "Dune", "buyer@example.com", "2", "24"} {
		if !contains(m.Body, want) {
			t.Errorf("notice body missing %q:\n%s", want, m.Body)
		}
	}
}

func TestBuildPromoMails(t *testing.T) {
	e := PromotionEvent{
		RecipientEmails: []string{"fan1@example.com", "fan2@example.com"},
		ProductId:       "33333333-3333-3333-3333-333333333333",
		ProductName:     "Dune",
		OriginalPrice:   30,
		DiscountedPrice: 24,
		PercentOff:      20,
		CategoryName:    "Sci-Fi",
	}
	mails := BuildPromoMails(e)
	if len(mails) != 2 {
		t.Fatalf("got %d mails, want 2", len(mails))
	}
	for i, want := range []string{"fan1@example.com", "fan2@example.com"} {
		if mails[i].To != want {
			t.Errorf("mails[%d].To = %q", i, want)
		}
	}
	for _, want := range []string{"Dune", "24", "20%", "Sci-Fi"} {
		if !contains(mails[0].Body, want) {
			t.Errorf("promo body missing %q:\n%s", want, mails[0].Body)
		}
	}
}

func contains(s, sub string) bool {
	return len(s) >= len(sub) && (s == sub || len(sub) == 0 || (len(s) > 0 && containsAt(s, sub)))
}

func containsAt(s, sub string) bool {
	for i := 0; i+len(sub) <= len(s); i++ {
		if s[i:i+len(sub)] == sub {
			return true
		}
	}
	return false
}
