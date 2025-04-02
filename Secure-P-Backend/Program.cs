var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers(o =>
{
    o.UseRoutePrefix(AppConstants.DefaultRoutePrefix);
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.ConsentCookieValue = "true";
    options.CheckConsentNeeded = context => AppConstants.EnableGDPR;
    options.ConsentCookie = new CookieBuilder
    {
        Name = AppConstants.DefaultLoginProvider + "_ConsentCookie",
        Expiration = TimeSpan.FromDays(90),
        IsEssential = true,
        SameSite = SameSiteMode.None,
        SecurePolicy = CookieSecurePolicy.Always,
        HttpOnly = false
    };
});

builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddUploadService<string>();

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Secure-P Backend";
    options.Version = "v1";
    options.Description = "Secure-P Backend API";
    options.DocumentName = "v1";

    options.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = Microsoft.Net.Http.Headers.HeaderNames.Authorization,
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}.",
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

builder.Services.Configure<JwtConfigures>(builder.Configuration.GetSection("Jwt")); // Configure JWT settings
var jwtConfigures = builder.Configuration.GetSection("Jwt").Get<JwtConfigures>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        /* options.Authority = jwtConfigures?.Authority ?? throw new InvalidOperationException("Authority is missing");
        options.Audience = jwtConfigures?.Audience ?? throw new InvalidOperationException("Audience is missing"); */

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseCORS();

app.UseRouting(); // Add routing middleware

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// app.MapIdentityApi<AppUser<string>>(); // Map identity API routes

app.MapControllers(); // Map controllers with group name "api"

app.Run();
