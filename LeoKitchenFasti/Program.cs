using LeoKitchenFasti.Data;
using LeoKitchenFasti.DTOs;
using LeoKitchenFasti.Services;
using LeoKitchenFasti.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using LeoKitchenFasti.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --- 1. BASE DE DATOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// --- 2. CONFIGURACIÓN DE JSON Y CONTROLADORES ---
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errorResponse = ApiResponse<object>.Error("Sintaxis incorrecta en la petición");
            return new BadRequestObjectResult(errorResponse);
        };
    });
builder.Services.AddSignalR(); // <- SignalR

// --- 3. CORS (Abierto para todos en desarrollo) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // TRUCO: Permite cualquier origen dinámicamente
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Mantiene vivas tus cookies HttpOnly del JWT
    });
});

// --- 4. INYECCIÓN DE DEPENDENCIAS (Servicios) ---
// Aquí registramos solo lo del MVP del restaurante
builder.Services.AddScoped<AuthService>(); // <-- Auntenticacion 

builder.Services.AddScoped<IUserService, UserService>(); // <-- Servicio de Usuarios

builder.Services.AddScoped<IOrderService, OrderService>(); // <-- Servicios de Ordens (SignalR)

// --- 5. SEGURIDAD: JWT Y COOKIES ---
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token)) context.Token = token;
                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                var userRoleClaim = context.Principal.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || userRoleClaim == null)
                {
                    context.Fail("Token corrupto");
                    return;
                }

                var userId = int.Parse(userIdClaim.Value);
                var tokenRole = userRoleClaim.Value;

                var user = await dbContext.Users
                    .AsNoTracking()
                    .Include(u => u.Rol)
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.IsActive, NombreRol = u.Rol.Nombre })
                    .FirstOrDefaultAsync();

                if (user == null || !user.IsActive)
                {
                    context.Fail("Tu cuenta ha sido desactivada.");
                    return;
                }

                if (user.NombreRol != tokenRole)
                {
                    context.Fail("Roles inconsistentes, vuelve a iniciar sesión");
                    return;
                }
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = context.AuthenticateFailure?.Message ?? "No estás autorizado",
                    data = (object)null
                });
                return context.Response.WriteAsync(jsonResponse);
            }
        };
    });

// --- 6. ESCUDO: RATE LIMITER (Blindaje DDoS) ---
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        var httpContext = context.HttpContext;
        httpContext.Response.ContentType = "application/json";
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(new
        {
            success = false,
            message = "Has excedido el límite de solicitudes permitidas. Por seguridad, espera un momento antes de reintentar.",
            data = (object)null
        });
        await httpContext.Response.WriteAsync(jsonResponse, cancellationToken: token);
    };

    options.AddPolicy("ProteccionAbuso", httpContext =>
    {
        var partitionKey = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? httpContext.Connection.RemoteIpAddress?.ToString()
                           ?? "cliente_general";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- MIDDLEWARES DEL PIPELINE ---
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PoliticaFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<LeoKitchenFasti.Hubs.RestaurantHub>("/restaurantHub"); // <--- 2. ABRE EL CANAL
app.MapHub<OrderHub>("/orderHub");

// --- INTERCEPTOR DE ARRANQUE (LOGO ASCII) ---
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(@"   _      ______ ____    _  _______ _______ _____ _    _ ______ _   _ ");
    Console.WriteLine(@"  | |    |  ____/ __ \  | |/ /_   _|__   __/ ____| |  | |  ____| \ | |");
    Console.WriteLine(@"  | |    | |__ | |  | | | ' /  | |    | | | |    | |__| | |__  |  \| |");
    Console.WriteLine(@"  | |    |  __|| |  | | |  <   | |    | | | |    |  __  |  __| | . ` |");
    Console.WriteLine(@"  | |____| |___| |__| | | . \ _| |_   | | | |____| |  | | |____| |\  |");
    Console.WriteLine(@"  |______|______\____/  |_|\_\_____|  |_|  \_____|_|  |_|______|_| \_|" + Environment.NewLine);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(@"                       ______       _____ _______ _____ ");
    Console.WriteLine(@"                      |  ____/\    / ____|__   __|_   _|");
    Console.WriteLine(@"                      | |__ /  \  | (___    | |    | |  ");
    Console.WriteLine(@"                      |  __/ /\ \  \___ \   | |    | |  ");
    Console.WriteLine(@"                      | | / ____ \ ____) |  | |   _| |_ ");
    Console.WriteLine(@"                      |_|/_/    \_\_____/   |_|  |_____|" + Environment.NewLine);
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("-----------------------------------------------------------------------------");
    Console.WriteLine("     >> Sistema de Comandas y KDS en Tiempo Real - LeoKitchenFasti <<");
    Console.WriteLine("     >> s4lm0.exe <<");
    Console.WriteLine("=============================================================================");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.Yellow;
    foreach (var url in app.Urls)
    {
        Console.WriteLine($"[+] Escuchando en: {url}");
    }
    Console.ResetColor();
});

app.Run();