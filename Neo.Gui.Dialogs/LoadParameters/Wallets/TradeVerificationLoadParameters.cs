﻿using System.Collections.Generic;
using Neo.Core;

namespace Neo.Gui.Dialogs.LoadParameters.Wallets
{
    public class TradeVerificationLoadParameters
    {
        public IEnumerable<TransactionOutput> TransactionOutputs { get; }

        public TradeVerificationLoadParameters(IEnumerable<TransactionOutput> outputs)
        {
            this.TransactionOutputs = outputs;
        }
    }
}
