using axionpro.api.Middlewares;
using axionpro.application;
using axionpro.infrastructure;
using axionpro.persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 🔥 ENV CHECK
    var isLocal = builder.Environment.IsDevelopment();

    // 🔥 Kestrel CONFIG (IMPORTANT)
    if (isLocal)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(7788); // 🔥 Device
            options.ListenAnyIP(5170); // 🔥 Swagger
        });
    }

    // ✅ Serilog
    builder.Host.UseSerilog();

    // ✅ JWT
    var jwtSettings = builder.Configuration.GetSection("JWTSettings");
    var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };
    });

    // ✅ SERVICES
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();

    // ✅ CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",    // Angular local
                "http://localhost:4201",    // React local (optional)
                "https://axion-pro.vercel.app",
                "https://axionpro-app.vercel.app/auth/login",
                "https://axionpro-app.vercel.app"


            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });
    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy("AllowAll", policy =>
    //    {
    //        policy.AllowAnyHeader()
    //              .AllowAnyMethod()
    //              .AllowAnyOrigin();
    //    });
    //});

    // ✅ Swagger
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
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Description = "Enter Bearer token"
        });
    });

    // 🔥 BUILD
    var app = builder.Build();

    // ❌ HTTPS only production
    if (!isLocal)
    {
        app.UseHttpsRedirection();
    }

    // ✅ Middleware pipeline
    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    // 🔥 IMPORTANT: Error handler BEFORE websocket safe
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // 🔥 WebSocket (Device)
    app.UseWebSockets();
    app.UseMiddleware<WebSocketMiddleware>();

    // 🔥 Tenant AFTER websocket
    app.UseMiddleware<TenantContextMiddleware>();

    app.MapControllers();

    // ✅ Swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    // 🔥 Production PORT (Render / VPS)
    if (!isLocal)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Urls.Add($"http://*:{port}");
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}