using axionpro.api.Common.Swagger;
using axionpro.api.Middlewares;
using axionpro.application;
using axionpro.application.DTOS.Configruations;
using axionpro.infrastructure;
using axionpro.persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
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

    // 🔥 ENV CHECK (IMPORTANT)
    var isLocal = builder.Environment.IsDevelopment();

    // 🔥 PORT CONFIG (FIXED)
    if (isLocal)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(7788); // ✅ Device ke liye
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

    // ✅ Services
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.Configure<EmailConfig>(
        builder.Configuration.GetSection("EmailConfig"));

    // ✅ Controllers + CORS
    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
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

    // ✅ Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Axion-Pro API",
            Version = "1.0"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Description = "Enter Bearer token"
        });

        c.OperationFilter<AuthorizeCheckOperationFilter>();
    });

    var app = builder.Build();

    // ✅ Static files
    app.UseStaticFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "wwwroot/uploads")),
        RequestPath = "/uploads"
    });

    // ❌ IMPORTANT: Device ke liye disable karo
    if (!isLocal)
    {
        app.UseHttpsRedirection(); // Render ke liye hi
    }

    app.UseCors("AllowAngularApp");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<ErrorHandlerMiddleware>();

    // 🔥 WebSocket (MOST IMPORTANT)
    app.UseWebSockets();
    app.UseMiddleware<WebSocketMiddleware>(); // FIRST

    // 🔥 Tenant baad me
    app.UseMiddleware<TenantContextMiddleware>();

    app.MapControllers();

    // 🔥 Render ke liye port
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