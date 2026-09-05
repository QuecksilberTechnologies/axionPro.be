// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Configures the AxionPro API host, authentication pipeline, and centralized HTTP error behavior.
// ================================================================

using axionpro.api.Middlewares;
using axionpro.api.Realtime;
using axionpro.api.Common.Swagger;
using axionpro.application;
using axionpro.application.Constants;
using axionpro.infrastructure;
using axionpro.persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Net;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Environment check
    var isLocal = builder.Environment.IsDevelopment();

    // Kestrel configuration for local execution
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
    // JWT configuration
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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"].FirstOrDefault();
                var isNotificationHubRequest = string.Equals(
                    context.HttpContext.Request.Path.Value,
                    SignalRRouteConstants.NotificationHub,
                    StringComparison.OrdinalIgnoreCase);

                // Security: query-string tokens are accepted only by the authenticated SignalR hub.
                if (isNotificationHubRequest && !string.IsNullOrWhiteSpace(accessToken))
                    context.Token = accessToken;

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Suppress the framework challenge body so all authentication failures share the AxionPro envelope.
                context.HandleResponse();
                return ErrorHandlerMiddleware.HandleExceptionAsync(
                    context.HttpContext,
                    HttpStatusCode.Unauthorized,
                    AppConstants.ErrorCodes.Unauthorized,
                    AppConstants.ErrorMessages.Unauthorized);
            },
            OnForbidden = context =>
            {
                return ErrorHandlerMiddleware.HandleExceptionAsync(
                    context.HttpContext,
                    HttpStatusCode.Forbidden,
                    AppConstants.ErrorCodes.Forbidden,
                    AppConstants.ErrorMessages.PermissionDenied);
            }
        };

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
    // Service registration
    // ============================
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAxionProSignalR();

    builder.Services.AddControllers();

    // ============================
    // CORS
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
            .AllowAnyMethod()
            // Browser SignalR transports require credentialed CORS; the existing explicit origins remain unchanged.
            .AllowCredentials();
        });
    });

    // ============================
    // Swagger
    // ============================
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AxionPro API",
            Version = "v1"
        });

        var apiXmlDocumentation = Path.Combine(AppContext.BaseDirectory, "axionpro.api.xml");
        if (File.Exists(apiXmlDocumentation))
        {
            c.IncludeXmlComments(apiXmlDocumentation, includeControllerXmlComments: true);
        }

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Paste the raw JWT access token."
        });

        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
        });

        c.OperationFilter<CommonErrorResponsesOperationFilter>();
    });

    // ============================
    // Build the application
    // ============================
    var app = builder.Build();

    // ============================
    // Production port
    // ============================
    if (!isLocal)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Urls.Add($"http://*:{port}");
        Console.WriteLine($"Application started on port {port}");
    }

    // ============================
    // Middleware pipeline
    // ============================

    // HTTPS only in production
    if (!isLocal)
        app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnablePersistAuthorization());

    app.UseCors("AllowFrontend");

    // Global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // WebSocket support
    app.UseWebSockets();

    // Tenant context
    app.UseMiddleware<TenantContextMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapAxionProSignalR();

    #region Application Started Logging

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        app.Logger.LogInformation(
            """
            ========================================
            AXIONPRO APPLICATION STARTED SUCCESSFULLY
            Environment : {EnvironmentName}
            SignalR Hub : {SignalRHubPath}
            ========================================
            """,
            app.Environment.EnvironmentName,
            SignalRRouteConstants.NotificationHub);
    });

    #endregion

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
