using axionpro.api.Common;
using axionpro.api.Common.Swagger;
using axionpro.api.Middlewares;
using axionpro.application;
using axionpro.infrastructure;
using axionpro.persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

    builder.WebHost.UseIISIntegration();

    // ✅ Serilog integration
    builder.Host.UseSerilog();

    // ✅ Read JWT settings
    var jwtSettings = builder.Configuration.GetSection("JWTSettings");
    var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
    var tokenLifetime = TimeSpan.Parse(jwtSettings["TokenLifetime"]);

    // ✅ Configure JWT Authentication
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
            ClockSkew = TimeSpan.Zero // ⏱️ No extra time for expiration skew
        };
    });

    // ✅ Add Services & DI
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    

    // ✅ Add Controllers & CORS
    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",    // Angular local
                "http://localhost:4201",    // React local (optional)
                "http://15.206.90.1" ,       // Deployed app
                "https://axion-pro.vercel.app"
                

            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });

    // ✅ Swagger with JWT Authorization
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xml in xmlFiles)
        {
            c.IncludeXmlComments(xml, includeControllerXmlComments: true);
        }

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Axion-Pro API",
            Version = "1.0"
        });

        c.SchemaFilter<NullSchemaFilter>();

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' followed by space and JWT token. \n\nExample: Bearer eyJhbGciOi..."
        });

        c.OperationFilter<AuthorizeCheckOperationFilter>();
    });

    // ✅ Build the app
    var app = builder.Build();
    // Enable static files
    app.UseStaticFiles(); // For wwwroot folder

    // Optional: agar aap specific folder ko custom path se expose karna chahte ho
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "wwwroot/uploads")),
            RequestPath = "/uploads"
    });

    // ✅ Middleware setup
    app.UseHttpsRedirection();

    app.UseCors("AllowAngularApp");

    app.UseAuthentication(); // 🔒 Important: before Authorization
    app.UseAuthorization();

    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();
    // 🔥 Tenant context middleware (AFTER auth)
    app.UseMiddleware<TenantContextMiddleware>();
    app.MapControllers();
    // check
  // //  // ✅ Dynamic port (for hosting like Heroku)w
       var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
     app.Urls.Add($"http://*:{port}");
   
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
