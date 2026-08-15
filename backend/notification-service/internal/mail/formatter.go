package mail

import "fmt"

// Payload mirror of the .NET PurchaseEvent (System.Text.Json PascalCase).

type PurchaseEvent struct {
	UserEmail     string         `json:"UserEmail"`
	Cart          CartOverView   `json:"Cart"`
	AuthorNotices []AuthorNotice `json:"AuthorNotices"`
}

type CartOverView struct {
	Id    string        `json:"Id"`
	Items []CartProduct `json:"Items"`
}

type CartProduct struct {
	Id      string          `json:"Id"`
	Product ProductOverview `json:"Product"`
	Amount  int             `json:"Amount"`
}

type ProductOverview struct {
	Id             string  `json:"Id"`
	Name           string  `json:"Name"`
	Price          float64 `json:"Price"`
	ImageUrl       string  `json:"ImageUrl"`
	CategoryName   string  `json:"CategoryName"`
	IsOnSale       bool    `json:"IsOnSale"`
	SalePercent    *int    `json:"SalePercent"`
	EffectivePrice float64 `json:"EffectivePrice"`
}

type AuthorNotice struct {
	AuthorEmail    string  `json:"AuthorEmail"`
	AuthorName     string  `json:"AuthorName"`
	ProductId      string  `json:"ProductId"`
	ProductName    string  `json:"ProductName"`
	Amount         int     `json:"Amount"`
	UnitPricePaid  float64 `json:"UnitPricePaid"`
	SalePercentOff *int    `json:"SalePercentOff"`
}

type PromotionEvent struct {
	RecipientEmails []string `json:"RecipientEmails"`
	ProductId       string   `json:"ProductId"`
	ProductName     string   `json:"ProductName"`
	OriginalPrice   float64  `json:"OriginalPrice"`
	DiscountedPrice float64  `json:"DiscountedPrice"`
	PercentOff      int      `json:"PercentOff"`
	CategoryName    string   `json:"CategoryName"`
}

func BuildReceipt(e PurchaseEvent) Mail {
	var body string
	var total float64
	for _, item := range e.Cart.Items {
		lineTotal := item.Product.EffectivePrice * float64(item.Amount)
		total += lineTotal
		body += fmt.Sprintf(
			"- %s x%d @ %.2f (was %.2f)\n",
			item.Product.Name, item.Amount, item.Product.EffectivePrice, item.Product.Price,
		)
	}
	body += fmt.Sprintf("\nTotal: %.2f", total)
	return Mail{
		To:      e.UserEmail,
		Subject: fmt.Sprintf("Order receipt #%s", e.Cart.Id),
		Body:    body,
	}
}

func BuildAuthorNotice(n AuthorNotice, buyerEmail string) Mail {
	body := fmt.Sprintf(
		"Dear %s, your book \"%s\" was sold: %d copy/copies at %.2f each (paid by %s).",
		n.AuthorName, n.ProductName, n.Amount, n.UnitPricePaid, buyerEmail,
	)
	return Mail{To: n.AuthorEmail, Subject: "You made a sale!", Body: body}
}

func BuildPromoMails(e PromotionEvent) []Mail {
	body := fmt.Sprintf(
		"%s is now %d%% off: %.2f instead of %.2f (category: %s).",
		e.ProductName, e.PercentOff, e.DiscountedPrice, e.OriginalPrice, e.CategoryName,
	)
	mails := make([]Mail, 0, len(e.RecipientEmails))
	for _, email := range e.RecipientEmails {
		mails = append(mails, Mail{To: email, Subject: "A book you loved is on sale!", Body: body})
	}
	return mails
}
