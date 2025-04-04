using Secure_P_Backend.Cors.Extensions;
using SecureP.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Services are registered in the ConfigureServices method inside Startup foldersservices.Configure<JwtConfigures>(context.Configuration.GetSection(AppConstants.JwtConfiguresSection)); // Configure JWT settings
builder.Services.Configure<JwtConfigures>(builder.Configuration.GetSection(AppConstants.JwtConfiguresSection)); // Configure JWT settings
var jwtConfigures = builder.Configuration.GetSection(AppConstants.JwtConfiguresSection).Get<JwtConfigures>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfigures?.Authority ?? throw new InvalidOperationException("Authority is missing"),
            ValidAudience = jwtConfigures?.Audience ?? throw new InvalidOperationException("Audience is missing"),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigures?.Key ?? throw new InvalidOperationException("Key is missing"))),
            ClockSkew = TimeSpan.Zero
        };
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var defaultPW = builder.Configuration["defaultPW"] ?? string.Empty;
    await SeedData<string>.InitializeAsync(services, defaultPW);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting(); // Add routing middleware

app.UseCors();

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// app.MapIdentityApi<AppUser<string>>(); // Map identity API routes

app.MapControllers(); // Map controllers with group name "api"

app.Run();
