﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Tenatus.API.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public virtual TraderSetting TraderSetting { get; set; } = new TraderSetting();
        public string TradingClientType { get; set; }
        public virtual ICollection<UserOrder> UserOrders { get; set; }
    }
}