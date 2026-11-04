using Bitacora.Application.Interfaces;
using Bitacora.Infrastructure.Data;
using Bitacora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar controladores y OpenAPI/Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // OpenAPI nativo en .NET 10

// 2. Registrar el DbContext con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Registrar el Repositorio para la Inyección de Dependencias
builder.Services.AddScoped<IRegistroRepository, RegistroRepository>();

var builderApp = builder.Build();

var app = builderApp;

// 4. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bitacora API v1");
    });
}

// Manejo global de excepciones nativo en ASP.NET Core
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ocurrió un error inesperado en el servidor.",
            Detail = "Por favor, inténtelo más tarde o contacte al administrador."
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
