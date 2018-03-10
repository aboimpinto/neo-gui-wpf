namespace Neo.UniversalWallet.Data
{
    public class ApplicationContext : IApplicationContext
    {
        public NetworkTypeEnum Network { get; set; }

        public WalletDto Wallet { get; set; }
    }
}
