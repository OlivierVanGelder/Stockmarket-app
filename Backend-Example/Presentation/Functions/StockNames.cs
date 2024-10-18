using Backend_Example.Logic.Stocks;
using DAL.BDaccess;

namespace Backend_Example.Functions
{
    public static class StockNames
    {
        public static void GetStockNames(this WebApplication app)
        {
            app.MapGet(
                    "/stocknames",
                    () =>
                    {
                        StockDAL stockDAL = new StockDAL();

                        return CandleStock.GetStockNames(stockDAL);
                    }
                )
                .WithName("GetStockNames")
                .WithOpenApi();
        }
    }
}
