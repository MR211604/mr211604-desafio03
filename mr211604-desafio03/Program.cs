using Microsoft.EntityFrameworkCore;
using mr211604_desafio03.Middleware;
using Pedidos.BL;
using Pedidos.DAL;
using Pedidos.DAL.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'ConnectionStrings:Default'.");

builder.Services.AddAccesoADatos(connectionString);
builder.Services.AddLogicaDeNegocio();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await AplicarMigracionesAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task AplicarMigracionesAsync(WebApplication app)
{
    const int maximoIntentos = 10;

    using var scope = app.Services.CreateScope();
    var contexto = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();

    for (var intento = 1; ; intento++)
    {
        try
        {
            await contexto.Database.MigrateAsync();
            return;
        }
        catch (Exception ex) when (intento < maximoIntentos)
        {
            app.Logger.LogWarning(ex, "No se pudo conectar a la base de datos (intento {Intento}/{Maximo}). Reintentando...", intento, maximoIntentos);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
