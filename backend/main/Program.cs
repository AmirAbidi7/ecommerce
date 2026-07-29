using EFCore.NamingConventions;
using main.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<Db>(
    (sp, options) =>
    {
        options
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention();
    }
);

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<Db>();
var canConnect = await db.Database.CanConnectAsync();

// await db.Database.MigrateAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
