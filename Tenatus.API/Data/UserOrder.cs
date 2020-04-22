using System;
using Tenatus.API.EnumTypes;

namespace Tenatus.API.Data
{
    public class UserOrder
    {
        public int Id { get; set; }
        public bool Buy { get; set; }
        public UserOrderActionType UserOrderActionType { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }
    }
}