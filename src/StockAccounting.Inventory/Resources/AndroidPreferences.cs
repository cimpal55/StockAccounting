using System.Collections.ObjectModel;
using Newtonsoft.Json;
using StockAccounting.Core.Data.Models.Data.ExternalData;

namespace StockAccounting.Inventory.Resources
{
    public sealed class AndroidPreferences
    {
        public AndroidPreferences()
        {
            Preferences.Set("SyncPass", "010907");
            Preferences.Set("ApiIp", "192.168.102.92");
            Preferences.Set("RestUrl", "http://192.168.102.92:83/api/");
        }

    }
}
