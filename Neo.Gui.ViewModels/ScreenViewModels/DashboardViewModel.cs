using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Neo.UI.Core.Data;
using Neo.UI.Core.Helpers.Extensions;
using Neo.UI.Core.Wallet;

namespace Neo.Gui.ViewModels.ScreenViewModels
{
    public class DashboardViewModel : ViewModelBase, ILoadable
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

        public ObservableCollection<WalletAccountDto> Accounts { get; private set; }
        #endregion

        #region Constructor 
        public DashboardViewModel(IWalletController walletController)
        {
            this._walletController = walletController;

            this.Accounts = new ObservableCollection<WalletAccountDto>();
        }
        #endregion

        #region ILoadable Implementation 
        public void OnLoad()
        {
            this.Accounts.AddRange(this._walletController.GetAccountsDto());

            this.AccountSelected = this.Accounts.First();          // TODO [AboimPinto]: In case of navigation to this screen and the WalletAccountDto is already selected, this logic is wrong.
        }
        #endregion
    }
}
