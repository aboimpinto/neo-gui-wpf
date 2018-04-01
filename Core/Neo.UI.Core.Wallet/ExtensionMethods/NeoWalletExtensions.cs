using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Neo.Core;
using Neo.Wallets;

namespace Neo.UI.Core.Wallet.ExtensionMethods
{
    public static class NeoWalletExtensions
    {
        public static IReadOnlyCollection<Coin> RetrieveUnspendCoins(this Wallets.Wallet wallet)
        {
            var coins = wallet
                .GetCoins()
                .Where(x => !x.State.HasFlag(CoinState.Spent))
                .ToList();

            return new ReadOnlyCollection<Coin>(coins);
        }
    }
}
