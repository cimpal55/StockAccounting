using System.ComponentModel;
using System.Configuration;

namespace StockAccounting.EmailBot.Models
{
    public class IMAPSettings : ConfigurationSection
    {
        public static IMAPSettings GetIMAPSettings()
        {
            return ConfigurationManager.GetSection("imapSettings") as IMAPSettings ?? new IMAPSettings();
        }

        [ConfigurationProperty("host", DefaultValue = "imap.gmail.com", IsRequired = true)]
        public string Host
        {
            get => (string)this["host"];
            set
            {
                value = (string)this["host"];
            }
        }

        [ConfigurationProperty("port", DefaultValue = 993, IsRequired = true)]
        public int Port
        {
            get => (int)this["port"];
            set
            {
                value = (int)this["port"];
            }
        }

        [ConfigurationProperty("email", IsRequired = true)]
        public string Email
        {
            get => (string)this["email"];
            set
            {
                value = (string)this["email"];
            }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get => (string)this["password"];
            set
            {
                value = (string)this["password"];
            }
        }

        [ConfigurationProperty("commands", IsRequired = true)]
        public string Commands
        {
            get => (string)this["commands"];
            set
            {
                value = (string)this["commands"];
            }
        }
    }
}
