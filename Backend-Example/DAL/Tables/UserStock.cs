using Microsoft.EntityFrameworkCore;

namespace DAL.Tables
{
    [PrimaryKey(nameof(UserId), nameof(StockId))]
    public class User_Stock
    {
        public string UserId { get; set; }
        public int StockId { get; set; }
        public int StockAmount { get; set; }
    }
}
