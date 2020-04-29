using System;
using System.ComponentModel.DataAnnotations;
using Tenatus.API.Data;
using Tenatus.API.Types;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class StrategyModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required] public string Stock { get; set; }
        [Required] public string UserOrderType { get; set; }
        [Required] public decimal Budget { get; set; }
        [Required] public string Type { get; set; }
        public decimal Percent { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public bool Active { get; set; }
        public string LastActive { get; set; }
        public string Created { get; set; }
    }
}