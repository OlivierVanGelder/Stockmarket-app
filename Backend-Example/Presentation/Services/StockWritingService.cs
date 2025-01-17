using Logic.Functions;
using Logic.Interfaces;

namespace Backend_Example.Services;

public class StockWritingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

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