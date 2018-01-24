﻿using System;
using System.Collections.Generic;

using Neo.UI.Core.Controllers.Interfaces;
using Neo.UI.Core.Data.TransactionParameters;

namespace Neo.UI.Core.Controllers
{
    public class TransactionInvokerFactory : ITransactionInvokerFactory
    {
        #region ITransactionInvokerFactory Implementation 
        public ITransactionInvoker GetTransactionInvoker(
            IWalletController walletController,
            IEnumerable<ITransactionInvoker> transactionInvokers,
            InvocationTransactionType invocationTransactionType,
            AssetRegistrationTransactionParameters assetRegistrationParameters,
            AssetTransferTransactionParameters assetTransferTransactionParameters,
            DeployContractTransactionParameters deployContractTransactionParameters,
            ElectionTransactionParameters electionTransactionParameters,
            VotingTransactionParameters votingTransactionParameters)
        {
            var configDictionary = new Dictionary<InvocationTransactionType, ITransactionConfiguration>
            {
                { invocationTransactionType, new AssetRegistrationTransactionConfiguration { WalletController = walletController, InvocationTransactionType = invocationTransactionType, AssetRegistrationTransactionParameters = assetRegistrationParameters } }
            };

            foreach(var invoker in transactionInvokers)
            {
                if (invoker.IsValid(invocationTransactionType))
                {
                    invoker.Configuration = configDictionary[invocationTransactionType];
                    invoker.GenerateTransaction();
                    return invoker;
                }
            }

            throw new InvalidOperationException($"Strategy for {invocationTransactionType.ToString()} not found.");
        }
        #endregion
    }
}
