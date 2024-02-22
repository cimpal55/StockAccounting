using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Api.Repositories;
using StockAccounting.Api.Utils.ServiceRegistration;
using Serilog;
using StockAccounting.Core.Data.DbAccess;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("Default");

    return options
        .UseSqlServer(connectionString)
        .UseDefaultLogging(provider);
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStockAccountingServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
