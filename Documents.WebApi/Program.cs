using Documents.Database.DependencyInjection;
using Documents.EndPoints;
using Documents.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

await app.Services.ApplyMigrationsAsync();

app.AddEndPoints();

app.Run();