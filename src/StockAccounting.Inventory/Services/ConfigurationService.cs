using StockAccounting.Inventory.Resources;
using StockAccounting.Inventory.Services.Interfaces;

namespace StockAccounting.Inventory.Services
{
    internal sealed class ConfigurationService : IConfiguration
    {
        public void SetPreferences()
        {
            _ = new AndroidPreferences();
        }
    }
}