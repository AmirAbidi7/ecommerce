using main.Config;
using main.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace main.Tests;

public abstract class TestBase
{
    protected IConfiguration config { get; }
    protected JwtService jwtService { get; }

    protected TestBase()
    {
        config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "Jwt:Key", "ThisIsASecretKeyThisIsASecretKey" },
                    { "Jwt:Issuer", "Ecommerce" },
                    { "Jwt:Audience", "EcommerceUsers" },
                    { "Jwt:DurationInMinutes", "1" },
                }
            )
            .Build();
        jwtService = new JwtService(config);
    }

    protected AppDb CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDb>()
            .UseInMemoryDatabase(databaseName ?? $"TestDb_{Guid.NewGuid()}")
            .Options;
        return new AppDb(options);
    }

    protected AppDb CreateSqliteContext()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDb>()
            .UseSqlite(connection)
            .Options;
        var db = new AppDb(options);
        db.Database.EnsureCreated();
        return db;
    }
}
