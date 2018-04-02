using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UI.Core.Data;

namespace Neo.Gui.ViewModels.ScreenViewModels
{
    public class TokenDetailsViewModel : ViewModelBase
    {
        #region Private Fields
        private AssetBalanceDto _assetDetails;
        #endregion

        #region Public Properties 
        public AssetBalanceDto AssetDetails
        {
            get => this._assetDetails;
            set
            {
                this._assetDetails = value;
                this.RaisePropertyChanged();
            }
        }

        public RelayCommand AssetSelectionCommand { get; private set; }
        #endregion

        #region Constructor
        public TokenDetailsViewModel()
        {
            this.AssetSelectionCommand = new RelayCommand(this.HandleAssetSelection);
        }
        #endregion

        #region Private Methods 
        private void HandleAssetSelection()
        {
        }
        #endregion
    }
}
