using System.Configuration;

namespace StockAccounting.EmailBot.Models
{
    public class SMTPSettings : ConfigurationSection
    {
        public static SMTPSettings GetSMTPSettings()
        {
            return ConfigurationManager.GetSection("smtpSettings") as SMTPSettings ?? new SMTPSettings();
        }

        [ConfigurationProperty("host", DefaultValue = "smtp.gmail.com")]
        public string Host
        {
            get => (string)this["host"];
            set
            {
                value = (string)this["host"];
            }
        }

        [ConfigurationProperty("port", DefaultValue = 587)]
        public int Port
        {
            get => (int)this["port"];
            set
            {
                value = (int)this["port"];
            }
        }

        [ConfigurationProperty("enableSsl", DefaultValue = true)]
        public bool EnableSsl
        {
            get => (bool)this["enableSsl"];
            set
            {
                value = (bool)this["enableSsl"];
            }
        }

        [ConfigurationProperty("isBodyHtml")]
        public bool IsBodyHtml
        {
            get => (bool)this["isBodyHtml"];
            set
            {
                value = (bool)this["isBodyHtml"];
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

    }
}
