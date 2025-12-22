using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Implementations;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// 1. CONTROLLERS + JSON
// ===============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ===============================================
// 2. CORS
// ===============================================
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://localhost:44353")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ===============================================
// 3. DATABASE
// ===============================================
builder.Services.AddDbContext<QuizGameContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===============================================
// 4. JWT - ✅ ĐÃ SỬA
// ===============================================
var jwtKey = builder.Configuration["Jwt:Key"]!;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // ✅ HỖ TRỢ JWT QUA QUERY STRING CHO WEBSOCKET
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                {
                    context.Token = accessToken;
                    Console.WriteLine($"[JWT] Token received from query string for path: {path}");
                }

                return Task.CompletedTask;
            }
        };
    });

// ===============================================
// 5. SWAGGER
// ===============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QUIZ_GAME_WEB API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// ===============================================
// 6. DI
// ===============================================
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITranDauRepository, TranDauRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IOnlineMatchService, OnlineMatchService>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IRewardService, RewardService>();

builder.Services.AddSingleton<ISocketGameServer, SocketGameServer>();

var app = builder.Build();

//// ===============================================
//// 7. MIGRATION
//// ===============================================
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<QuizGameContext>();
//    context.Database.Migrate();
//}

// ===============================================
// 8. PIPELINE - ✅ ĐÃ SỬA
// ===============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

// ✅ QUAN TRỌNG: UseAuthentication PHẢI ĐẶT TRƯỚC UseWebSockets
app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapControllers();

// ===============================================
// 9. MAP WEBSOCKET - ✅ ĐÃ SỬA
// ===============================================
app.Map("/ws/game", async (HttpContext context) =>
{
    var server = context.RequestServices.GetRequiredService<ISocketGameServer>();

    if (context.WebSockets.IsWebSocketRequest)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await server.Handle(context);
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.MapControllers();
app.Run();