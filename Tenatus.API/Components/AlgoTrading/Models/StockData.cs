﻿﻿using System;

 namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class StockData
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public decimal CurrentPrice { get; set; }
        public string Stock { get; set; }
        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        
    }
}