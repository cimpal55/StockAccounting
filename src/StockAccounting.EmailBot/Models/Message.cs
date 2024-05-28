using System.Collections.Generic;
using System;

namespace StockAccounting.EmailBot.Models
{
    public sealed class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public DateTime DateTime { get; set; }
        public string MsgID { get; set; }
    }
}
