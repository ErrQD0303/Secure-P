using SecureP.Identity.Models;
using SecureP.Service.TokenService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers(); // Add support for API controllers

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Secure-P Backend";
    options.Version = "v1";
    options.Description = "Secure-P Backend API";
    options.DocumentName = "v1";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(configure =>
    {
        configure.DocumentTitle = "Secure-P Backend API";
        configure.Path = "/swagger";
        configure.DocumentPath = "/swagger/{documentName}/swagger.json";
        configure.DocExpansion = "list";
    });
}

app.UseHttpsRedirection();

app.UseRouting(); // Add routing middleware

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// app.MapIdentityApi<AppUser<string>>(); // Map identity API routes

app.MapControllers(); // Map controller routes

app.Run();
