using Neo.UI.Core.Data.Enums;

namespace Neo.UI.Core.Data
{
    public class WalletAccountDto
    {
        public string Label { get; private set; }

        public string Address { get; private set; }

        public string ScriptHash { get; private set; }

        public string PublicKey { get; private set; }

        public AccountType AccountType { get; private set; }

        public WalletAccountDto(
            string label, 
            string address, 
            string scriptHash, 
            string publicKey,
            AccountType accountType)
        {
            this.Label = label;
            this.Address = address;
            this.ScriptHash = scriptHash;
            this.PublicKey = publicKey;
            this.AccountType = accountType;
        }
    }
}
