using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Tables;

[PrimaryKey(nameof(UserId), nameof(StockId))]
public class User_Stock
{
    [MaxLength(100)]
    public string UserId { get; set; } = "";
    public int StockId { get; set; }
    public int StockAmount { get; set; }
}
