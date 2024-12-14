using System.Text;
using Backend_Example.Controllers;
using Logic.Stocks;
using DAL.DbAccess;
using DAL.Tables;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var conn = Environment.GetEnvironmentVariable("ConnectionString");
builder.Services.AddDbContext<DbStockEngine>(options =>
    options.UseSqlServer(conn ?? builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        b =>
        {
            b
                .WithOrigins("http://localhost:3000", "http://localhost")
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
                if (context.Exception.InnerException == null)
                {
                    return Task.CompletedTask; }
                
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var responseMessage = new { message = "Authentication failed" };
                return context.Response.WriteAsJsonAsync(responseMessage);

            },
            OnChallenge = context =>
            {
                if (context.Response.HasStarted)
                {
                    return Task.CompletedTask;
                }
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var responseMessage = new { message = "Authentication required" };
                return context.Response.WriteAsJsonAsync(responseMessage);
            }
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")
            )
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

builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IStockDAal, StockDal>();
builder.Services.AddScoped<LineStock>();

builder.Services.AddWebSockets(_ => {});

var app = builder.Build();

app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DbStockEngine>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.NewManagerUiController();
app.NewStockController();
app.NewUserController(builder.Configuration);

app.Run();
