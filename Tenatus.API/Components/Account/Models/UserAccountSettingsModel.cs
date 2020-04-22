namespace Tenatus.API.Components.Account.Models
{
    public class UserAccountSettingsModel
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string TradingClientType { get; set; }
        public string AccountName { get; set; }
    }
}