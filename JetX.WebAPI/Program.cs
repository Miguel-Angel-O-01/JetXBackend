var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
// Registramos el puente de Supabase bajo el patrón Repository para Clean Architecture

builder.Services.AddScoped<JetX.Application.Interfaces.IJetRepository, JetX.Infrastructure.Repositories.SupabaseJetRepository>();

// Registramos el repositorio de rutas aéreas para la inyección de dependencias
builder.Services.AddScoped<JetX.Application.Interfaces.IRouteRepository, JetX.Infrastructure.Repositories.SupabaseRouteRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
