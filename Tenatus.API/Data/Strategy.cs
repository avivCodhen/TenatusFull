using Tenatus.API.Types;

namespace Tenatus.API.Data
{
    public abstract class Strategy
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Stock { get; set; }
        public UserOrderType UserOrderType { get; set; }
        public decimal Budget { get; set; }
    }
}