using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Neo.Gui.ViewModels.ExtensionMethods;
using Neo.Gui.ViewModels.Messages;
using Neo.UI.Core.Data;
using Neo.UI.Core.Helpers.Extensions;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UI.Core.Wallet;

namespace Neo.Gui.ViewModels.ScreenViewModels
{
    public class DashboardViewModel : 
        ViewModelBase, 
        ILoadable,
        IMessageHandler<NewBlockReceivedMessage>
    {
        #region Private Fields 
        private readonly IWalletController _walletController;
        private WalletAccountDto _accountSelected;
        #endregion

        #region Public Properties 
        public WalletAccountDto AccountSelected
        {
            get => this._accountSelected;
            set
            {
                this._accountSelected = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<WalletAccountDto> Accounts { get; }

        public TokenDetailsViewModel NeoTokenDetailsViewModel { get; }

        public TokenDetailsViewModel GasTokenDetailsViewModel { get; }

        public ObservableCollection<TransactionDto> LastTransactions { get; }
        #endregion

        #region Constructor 
        public DashboardViewModel(IMessageSubscriber messageSubscriber, IWalletController walletController)
        {
            this._walletController = walletController;

            messageSubscriber.Subscribe(this);

            this.Accounts = new ObservableCollection<WalletAccountDto>();
            this.LastTransactions = new ObservableCollection<TransactionDto>();
            this.NeoTokenDetailsViewModel = new TokenDetailsViewModel();
            this.GasTokenDetailsViewModel = new TokenDetailsViewModel();
        }
        #endregion

        #region ILoadable Implementation 
        public void OnLoad()
        {
            this.Accounts.AddRange(this._walletController.GetAccountsDto());
            this.AccountSelected = this.Accounts.First();          // TODO [AboimPinto]: In case of navigation to this screen and the WalletAccountDto is already selected, this logic is wrong.

            this.RefreshTokensBalance();
            this.RefreshTransactionList();
        }
        #endregion

        #region IMessageHandler Implementation 
        public void HandleMessage(NewBlockReceivedMessage message)
        {
            this.RefreshTokensBalance();
            this.RefreshTransactionList();
        }
        #endregion

        #region Private Methods
        private void RefreshTokensBalance()
        {
            var assetsBalance = this._walletController.GetAccountAssetBalanced(this.AccountSelected);
            this.NeoTokenDetailsViewModel.AssetDetails = assetsBalance.RetrieveNeoToken();
            this.GasTokenDetailsViewModel.AssetDetails = assetsBalance.RetrieveGasToken();
        }

        private async void RefreshTransactionList()
        {
            var transactions = await this._walletController.ListLastTransactions(this.AccountSelected);

            this.LastTransactions.Clear();

            foreach (var transaction in transactions)
            {
                this.LastTransactions.Add(transaction);
            }
        }
        #endregion
    }
}
