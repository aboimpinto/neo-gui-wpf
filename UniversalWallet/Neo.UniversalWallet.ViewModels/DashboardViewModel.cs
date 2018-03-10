using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UniversalWallet.Data;
using Neo.UniversalWallet.Model;
using Neo.UniversalWallet.ViewModels.Helpers;
using Neo.UniversalWallet.ViewModels.Helpers.Messages;

namespace Neo.UniversalWallet.ViewModels
{
    public class DashboardViewModel : ViewModelBase, ILoadable
    {
        #region Private Fields 
        private readonly INodeModel _nodeModel;
        private readonly IApplicationContext _applicationContext;

        private string _selectedAsset;
        #endregion

        #region Public Properties 
        public ObservableCollection<string> Assets { get; }

        public string SelectedAsset
        {
            get => this._selectedAsset;
            set
            {
                this._selectedAsset = value;
                this.RaisePropertyChanged();
            }
        }

        public RelayCommand<string> AssetSelectionCommand { get; private set; }
        #endregion

        #region Constructor 
        public DashboardViewModel(INodeModel nodeModel, IApplicationContext applicationContext)
        {
            this._nodeModel = nodeModel;
            this._applicationContext = applicationContext;

            this.Assets = new ObservableCollection<string>();

            this.AssetSelectionCommand = new RelayCommand<string>(this.HandleAssetSelection);
        }
        #endregion

        #region ILoadable Implementation 
        public void OnLoad()
        {
            this.Assets.Add("NEO"); 
            this.Assets.Add("GAS");
            this.Assets.Add("RPX");

            this.SelectedAsset = this.Assets.First();
        }
        #endregion

        #region Private Methods 
        private void HandleAssetSelection(string assetId)
        {
            MessengerInstance.Send(new NavigationMessage("AssetView"));
        }
        #endregion
    }
}
