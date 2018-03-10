using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UniversalWallet.Data;
using Neo.UniversalWallet.ViewModels.Helpers.Messages;
using Newtonsoft.Json;

namespace Neo.UniversalWallet.ViewModels
{
    public class LoadWalletViewModel : ViewModelBase
    {
        #region Private Fields 
        private readonly IApplicationContext _applicationContext;
        #endregion

        #region Public Properties 
        public ObservableCollection<string> Networks { get; private set; }

        public string SelectedNetwork { get; set; }

        public RelayCommand UnlockWalletCommand { get; private set; }
        #endregion

        #region Constructor
        public LoadWalletViewModel(IApplicationContext applicationContext)
        {
            this._applicationContext = applicationContext;

            this.Networks = new ObservableCollection<string> { "Mainnet", "Testnet", "CoZ Testnet", "Privatenet" };
            this.SelectedNetwork = this.Networks.First();

            this.UnlockWalletCommand = new RelayCommand(this.HandleUnlockWallet);
        }
        #endregion

        #region Private Methods 
        private void HandleUnlockWallet()
        {
            var wallet = JsonConvert.DeserializeObject<WalletDto>(File.ReadAllText(@"PrivateNetWallet.json"));
            this._applicationContext.Wallet = wallet;

            MessengerInstance.Send(new NavigationMessage("DashboardView"));
        }
        #endregion
    }
}
