package consumer

import (
	"context"
	"errors"
	"log/slog"
	"time"

	"github.com/twmb/franz-go/pkg/kgo"
)

func New(cfg Config) (*kgo.Client, error) {
	client, err := kgo.NewClient(
		kgo.SeedBrokers(cfg.Bootstrap),
		kgo.ConsumerGroup(cfg.Group),
		kgo.ConsumeTopics(cfg.Topics...),
		kgo.RecordPartitioner(kgo.StickyPartitioner()),
	)
	if err != nil {
		return nil, err
	}
	return client, nil
}

func Run(
	ctx context.Context,
	client *kgo.Client,
	handle func(ctx context.Context, topic string, value []byte) error,
	logger *slog.Logger,
) error {
	defer client.Close()
	for {
		if err := ctx.Err(); err != nil {
			return nil
		}
		fetches := client.PollFetches(ctx)
		if errors.Is(fetches.Err(), context.Canceled) {
			return nil
		}
		if err := fetches.Err(); err != nil {
			logger.Error("poll error", "error", err)
			time.Sleep(time.Second)
			continue
		}
		fetches.EachRecord(func(record *kgo.Record) {
			if err := handle(ctx, record.Topic, record.Value); err != nil {
				logger.Error("handle failed", "topic", record.Topic, "error", err)
			}
		})
	}
}