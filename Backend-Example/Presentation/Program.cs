using System.Text;
using Backend_Example.Controllers;
using Backend_Example.Data.BDaccess;
using Backend_Example.Logic.Stocks;
using DAL.BDaccess;
using DAL.Tables;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Set up services based on environment
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (environment == "Test")
{
    // Use SQLite In-Memory Database for Test environment
    builder.Services.AddDbContext<DbStockEngine>(options =>
        options.UseSqlite("DataSource=:memory:")
    ); // In-memory database
}
else
{
    // Use SQL Server for non-Test environments
    builder.Services.AddDbContext<DbStockEngine>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    ); // Connection string from appsettings
}

// Add other services and configurations
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000") // Frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

builder
    .Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DbStockEngine>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception != null)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var responseMessage = new { message = "Authentication failed" };
                    return context.Response.WriteAsJsonAsync(responseMessage);
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var responseMessage = new { message = "Authentication required" };
                    return context.Response.WriteAsJsonAsync(responseMessage);
                }
                return Task.CompletedTask;
            },
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
        };
    });

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddScoped<IUserDAL, UserDAL>();
builder.Services.AddScoped<IStockDAL, StockDAL>();
builder.Services.AddScoped<LineStock>();

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    )
);

builder.Services.AddWebSockets(options => { });

var app = builder.Build();

// Seed the database if in test mode (for in-memory testing)
if (builder.Environment.IsDevelopment() || environment == "Test")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        SeedData.Initialize(services);
    }
}

// Development-specific setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowFrontend");

// Enable WebSockets
app.UseWebSockets();
app.Stockcontroller(builder.Configuration);
app.Usercontroller(builder.Configuration);

app.Run();
