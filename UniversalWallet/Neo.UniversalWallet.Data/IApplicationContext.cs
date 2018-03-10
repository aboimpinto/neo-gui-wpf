namespace Neo.UniversalWallet.Data
{
    public interface IApplicationContext
    {
        NetworkTypeEnum Network { get; set; }

        WalletDto Wallet { get; set; }
    }
}
