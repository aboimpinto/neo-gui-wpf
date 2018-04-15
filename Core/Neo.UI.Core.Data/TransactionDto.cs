using System;

namespace Neo.UI.Core.Data
{
    public class TransactionDto
    {
        public DateTime TransactionTimeStamp { get; set; }

        public string TransactionHash { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }

        public string Token { get; set; }

        public string Value { get; set; }
    }
}
