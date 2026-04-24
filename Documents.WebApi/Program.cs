using Documents.Database.DependencyInjection;
using Documents.EndPoints;
using Documents.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();

await app.Services.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.Use(async (context, next) =>
//{
//    app.Logger.LogInformation("[REQUEST] {Method} {Path}", context.Request.Method, context.Request.Path);
//    await next();
//    app.Logger.LogInformation("[RESPONSE] {StatusCode}", context.Response.StatusCode);
//});


app.AddEndPoints();

app.Run();