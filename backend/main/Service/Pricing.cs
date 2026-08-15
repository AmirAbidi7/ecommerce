using main.Entity;

namespace main.Service;

public static class Pricing
{
    public static float EffectivePrice(float price, int percentOff) =>
        price * (1 - percentOff / 100f);

    public static bool IsActive(Sale sale, DateTime now) =>
        sale.StartsAt <= now && now <= sale.EndsAt;
}