using System.Configuration;

namespace StockAccounting.EmailBot.Models
{
    public class OAuth2Credentials : ConfigurationSection
    {
        public static OAuth2Credentials GetOAuth2Credentials()
        {
            return ConfigurationManager.GetSection("oAuth2Credentials") as OAuth2Credentials ?? new OAuth2Credentials();
        }

        [ConfigurationProperty("clientId", IsRequired = true)]
        public string ClientId
        {
            get => (string)this["clientId"];
            set
            {
                value = (string)this["clientId"];
            }
        }

        [ConfigurationProperty("tenantId", IsRequired = true)]
        public string TenantId
        {
            get => (string)this["tenantId"];
            set
            {
                value = (string)this["tenantId"];
            }
        }

        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret
        {
            get => (string)this["secret"];
            set
            {
                value = (string)this["secret"];
            }
        }
    }
}
