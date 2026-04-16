using axionpro.api.Middlewares;
using axionpro.application;
using axionpro.infrastructure;
using axionpro.persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
 
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ✅ ENV CHECK
    var isLocal = builder.Environment.IsDevelopment();

    // ✅ Kestrel Config (LOCAL ONLY)
    if (isLocal)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(7788); // Device
            options.ListenAnyIP(5170); // Swagger
        });

        Console.WriteLine($"Application started in LOCAL mode: {isLocal}");
    }

    // ============================
    // 🔐 JWT CONFIG (SAFE)
    // ============================
    var jwtSettings = builder.Configuration.GetSection("JWTSettings");

    var secret = jwtSettings["Secret"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    var tokenLifetime = jwtSettings["TokenLifetime"];

    if (string.IsNullOrWhiteSpace(secret))
        throw new Exception("JWT Secret missing");

    if (!TimeSpan.TryParse(tokenLifetime, out var tokenExpiry))
        throw new Exception("Invalid TokenLifetime format");

    var secretKey = Encoding.UTF8.GetBytes(secret);

    if (string.IsNullOrWhiteSpace(secret))
        throw new Exception("JWT Secret is missing in appsettings.json");

    

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !isLocal;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),

            ClockSkew = TimeSpan.Zero
        };
    });

    // ============================
    // 🧩 SERVICES
    // ============================
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();

    // ============================
    // 🌐 CORS
    // ============================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4201",
                "https://axion-pro.vercel.app",
                "https://axionpro-app.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });

    // ============================
    // 📘 SWAGGER
    // ============================
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AxionPro API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token"
        });

        //c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //{
        //    {
        //        new OpenApiSecurityScheme
        //        {
        //            Reference = new OpenApiReference
        //            {
        //                Type = ReferenceType.SecurityScheme,
        //                Id = "Bearer"
        //            }
        //        },
        //        new string[] {}
        //    }
        //});
    });

    // ============================
    // 🚀 BUILD
    // ============================
    var app = builder.Build();

    // ============================
    // 🌍 PRODUCTION PORT
    // ============================
    if (!isLocal)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Urls.Add($"http://*:{port}");
        Console.WriteLine($"Application started on port {port}");
    }

    // ============================
    // 🔐 MIDDLEWARE PIPELINE
    // ============================

    // ❌ HTTPS only in production
    if (!isLocal)
        app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("AllowFrontend");

    // 🔥 Global Error Handler (FIRST)
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // 🔥 WebSocket
    app.UseWebSockets();
    app.UseMiddleware<WebSocketMiddleware>();

    // 🔥 Tenant Context
    app.UseMiddleware<TenantContextMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Application start-up failed: {ex.Message}");
}
finally
{
    Console.WriteLine("🛑 Application shutting down...");
}