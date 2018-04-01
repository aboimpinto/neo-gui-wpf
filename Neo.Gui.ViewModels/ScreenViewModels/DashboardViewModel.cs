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
        private AssetBalanceDto _neoTokenDetails;
        private AssetBalanceDto _gasTokenDetails;
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

        public AssetBalanceDto NeoTokenDetails
        {
            get => this._neoTokenDetails;
            set
            {
                this._neoTokenDetails = value;
                this.RaisePropertyChanged();
            }
        }

        public AssetBalanceDto GasTokenDetails
        {
            get => this._gasTokenDetails;
            set
            {
                this._gasTokenDetails = value;
                this.RaisePropertyChanged();
            }
        }
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

            var assetsBalance = this._walletController.GetAccountAssetBalanced(this.AccountSelected);

            this.NeoTokenDetails = assetsBalance.Single(x => x.AssetType == AssetTypeDto.GoverningToken);
            this.GasTokenDetails = assetsBalance.Single(x => x.AssetType == AssetTypeDto.UtilityToken);
        }
        #endregion
    }
}
