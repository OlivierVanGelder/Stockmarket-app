using Backend_Example;
using Backend_Example.Controllers;
using Backend_Example.Logic.Classes;
using DAL.BDaccess;
using Logic.Functions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Temporary CORS policy to allow all origins
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()
    )
);

StockDAL stockDAL = new StockDAL();

//StockWritingInterval stockWritingInterval = new StockWritingInterval(20, stockDAL);

//StockDeletingInterval stockDeletingInterval = new StockDeletingInterval(5, stockDAL);

var app = builder.Build();

app.UseCors(policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.UseWebSockets(
    new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromSeconds(120), // Adjust as needed
    }
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.ClientUIcontroller();

app.Run();
