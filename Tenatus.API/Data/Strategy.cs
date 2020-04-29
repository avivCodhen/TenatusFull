using System;
using Tenatus.API.Types;

namespace Tenatus.API.Data
{
    public abstract class Strategy
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Stock { get; set; }
        public string UserOrderType { get; set; }
        public decimal Budget { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset LastActive { get; set; }
    }
}