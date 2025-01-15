using System.Formats.Asn1;
using DAL.DbAccess;
using Logic.Functions;
using Logic.Interfaces;
using Microsoft.OpenApi.Writers;

public class StockWritingService : BackgroundService
{
    private IServiceProvider _serviceProvider;

    public StockWritingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var stockDal = scope.ServiceProvider.GetRequiredService<IStockDal>();
            await StockWritingInterval.WriteStocks(stockDal);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}