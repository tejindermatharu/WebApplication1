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

        public string BuySell { get; set; }
        public string Type { get; set; }
    }
}
