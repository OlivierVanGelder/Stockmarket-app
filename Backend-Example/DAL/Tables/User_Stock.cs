using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tables
{
    [PrimaryKey(nameof(UserId), nameof(StockId))]
    public class User_Stock
    {
        public int UserId { get; set; }
        public int StockId { get; set; }
        public int StockAmount { get; set; }
    }
}
