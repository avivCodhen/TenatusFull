using System.Collections.Generic;

namespace Tenatus.API.Data
{
    public class TraderSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}