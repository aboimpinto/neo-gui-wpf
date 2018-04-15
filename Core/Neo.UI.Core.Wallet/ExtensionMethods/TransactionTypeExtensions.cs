using System;
using Neo.Core;
using Neo.UI.Core.Data;

namespace Neo.UI.Core.Wallet.ExtensionMethods
{
    public static class TransactionTypeExtensions
    {
        public static TransactionTypeEnum ToTransactionTypeEnum(this TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.ContractTransaction:
                    return TransactionTypeEnum.ContractTransaction;
                case TransactionType.ClaimTransaction:
                    return TransactionTypeEnum.ClaimTransaction;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
