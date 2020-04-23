namespace Tenatus.API.Data
{
    public class Stock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TradeSettingId { get; set; }
        public virtual TraderSetting TraderSetting { get; set; }
    }
}