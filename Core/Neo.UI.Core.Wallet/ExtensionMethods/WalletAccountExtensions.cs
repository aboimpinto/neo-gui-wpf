using Neo.UI.Core.Data;
using Neo.UI.Core.Data.Enums;
using Neo.Wallets;

namespace Neo.UI.Core.Wallet.ExtensionMethods
{
    public static class WalletAccountExtensions
    {
        public static WalletAccountDto ToWalletAccountDto(this WalletAccount walletAccount)
        {
            var label = string.IsNullOrEmpty(walletAccount.Label) ?
                walletAccount.Address : 
                walletAccount.Label;

            return new WalletAccountDto(
                label, 
                walletAccount.Address,
                walletAccount.ScriptHash.ToString(),
                walletAccount.GetKey().PublicKey.ToString(),
                walletAccount.GetAccountType());
        }

        public static AccountType GetAccountType(this WalletAccount walletAccount)
        {
            var accountType = AccountType.NonStandard;

            if (walletAccount.WatchOnly)
            {
                accountType = AccountType.WatchOnly;
            }

            if (walletAccount.Contract.IsStandard)
            {
                accountType = AccountType.Standard;
            }

            return accountType;
        }
    }
}
