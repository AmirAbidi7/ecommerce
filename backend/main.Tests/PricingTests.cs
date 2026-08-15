using main.Entity;
using main.Service;

namespace main.Tests;

public class PricingTests
{
    [Theory]
    [InlineData(100f, 0, 100f)]
    [InlineData(100f, 25, 75f)]
    [InlineData(50f, 100, 0f)]
    public void EffectivePrice_ShouldApplyPercent(float price, int percent, float expected)
    {
        Assert.Equal(expected, Pricing.EffectivePrice(price, percent));
    }

    [Fact]
    public void IsActive_ShouldBeTrueWhenNowWithinRange()
    {
        var sale = new Sale { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), PercentOff = 10,
            StartsAt = DateTime.UtcNow.AddDays(-1), EndsAt = DateTime.UtcNow.AddDays(1),
            CreatedBy = Guid.NewGuid(), Product = null! };
        Assert.True(Pricing.IsActive(sale, DateTime.UtcNow));
    }

    [Fact]
    public void IsActive_ShouldBeFalseWhenExpired()
    {
        var sale = new Sale { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), PercentOff = 10,
            StartsAt = DateTime.UtcNow.AddDays(-2), EndsAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid(), Product = null! };
        Assert.False(Pricing.IsActive(sale, DateTime.UtcNow));
    }
}