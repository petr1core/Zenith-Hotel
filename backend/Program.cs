using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hotel_MVP.Data;
using Hotel_MVP.Services;
using Hotel_MVP;  // Добавляем using для JsonDateTimeConverter
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.FileProviders;
using Hotel_MVP.Utils;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку переменных окружения
builder.Configuration.AddEnvironmentVariables();
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Configuration["Jwt:Key"] = jwtKey;
}

// Настройка пути для статических файлов
var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var uploadsPath = Path.Combine(wwwrootPath, "uploads");
Console.WriteLine($"Rooms photos path: {uploadsPath}");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    Console.WriteLine($"Created directory: {uploadsPath}");
}

// Проверяем существование файлов в папке uploads
if (Directory.Exists(uploadsPath))
{
    var files = Directory.GetFiles(uploadsPath);
    Console.WriteLine($"Files in uploads directory: {string.Join(", ", files)}");
}

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/hotel-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel API",
        Version = "v1",
        Description = "API для управления отелем",
        Contact = new OpenApiContact
        {
            Name = "Hotel Support",
            Email = "support@hotel.com"
        }
    });

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Включаем XML-комментарии
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(); // Включаем логирование SQL-запросов
    options.EnableDetailedErrors(); // Включаем подробные сообщения об ошибках
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Обновленные настройки CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAndSwagger", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:7297")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddSerilog();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            ClockSkew = TimeSpan.Zero // Минус смещение в 5 минут
        };

        // Добавляем обработку ошибок валидации токена
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Management", policy =>
        policy.RequireRole("Admin", "Manager"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel API V1");
        c.RoutePrefix = "swagger"; // Устанавливаем путь к Swagger UI
        c.DocumentTitle = "Hotel API Documentation";
        c.DefaultModelsExpandDepth(-1); // Скрываем модели по умолчанию
        c.EnableDeepLinking(); // Включаем глубокие ссылки
        c.DisplayRequestDuration(); // Показываем длительность запросов
        c.EnableValidator(); // Включаем валидатор
        c.DocExpansion(DocExpansion.None); // Сворачиваем все операции по умолчанию
    });
}

// Настраиваем обработку статических файлов
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    OnPrepareResponse = ctx =>
    {
        // Разрешаем CORS для статических файлов
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
    }
});

// Добавляем middleware для логирования запросов
app.Use(async (context, next) =>
{
    Log.Information($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Log.Information($"Response: {context.Response.StatusCode}");
});

// middleware
app.UseRouting();
app.UseCors("FrontendAndSwagger"); // Обновляем название политики
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// Добавляем обработку ошибок
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error($"Unhandled exception: {ex}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
    }
});

app.MapControllers(); // Для API контроллеров
app.MapGet("/", () => "Hotel Management API is running!"); // Базовый маршрут

try
{
    Log.Information("Starting web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}