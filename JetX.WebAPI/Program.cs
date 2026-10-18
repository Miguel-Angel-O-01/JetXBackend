using JetX.Application.Interfaces;
using JetX.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Inyección de repositorios
builder.Services.AddScoped<IJetRepository, SupabaseJetRepository>();
builder.Services.AddScoped<IRouteRepository,SupabaseRouteRepository>();

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
