using System.Collections.Generic;
using System.Linq;
using Neo.UI.Core.Data;
using Neo.Wallets;

namespace Neo.UI.Core.Wallet.ExtensionMethods
{
    public static class CoinExtensions
    {
        public static double CalculateTokenBalance(this IEnumerable<Coin> coins, WalletAccountDto account, string tokenScriptHash)
        {
            var scriptHash = UInt256.Parse(tokenScriptHash);
            var accountScriptHash = UInt160.Parse(account.ScriptHash);

            var balances = coins
                .Where(p => p.Output.AssetId.Equals(scriptHash))
                .GroupBy(p => p.Output.ScriptHash)
                .ToDictionary(p => p.Key, p => p.Sum(i => i.Output.Value));

            return balances.ContainsKey(accountScriptHash) ? 
                double.Parse(balances[accountScriptHash].ToString()) : 
                0;
        }
    }
}
