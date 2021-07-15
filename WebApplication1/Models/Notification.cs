using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Notification
    {
        public string Isin { get; set; }
        public string Counterparty { get; set; }
        public string Trader { get; set; }
        public string Sales { get; set; }
        public decimal Quote { get; set; }
        public decimal Yield { get; set; }
    }
}
