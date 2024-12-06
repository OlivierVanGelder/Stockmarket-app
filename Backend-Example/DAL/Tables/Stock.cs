using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tables
{
    public class Stock
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public int PriceInCents { get; set; }
    }
}
