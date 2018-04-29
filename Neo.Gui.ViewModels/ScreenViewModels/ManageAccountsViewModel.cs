using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UI.Core.Messaging.Interfaces;

namespace Neo.Gui.ViewModels.ScreenViewModels
{
    public class ManageAccountsViewModel : ViewModelBase
    {
        #region Private fields
        private readonly IMessagePublisher _messagePublisher;
        #endregion

        #region Public Properties 
        public RelayCommand BackCommand { get; set; }
        #endregion

        #region Constructor
        public ManageAccountsViewModel(IMessagePublisher messagePublisher)
        {
            this._messagePublisher = messagePublisher;

            this.BackCommand = new RelayCommand(this.HandleBackNaviation);
        }
        #endregion

        #region Private Methods 
        private void HandleBackNaviation()
        {
            this._messagePublisher.Publish(new NavigationMessage(ViewNames.DashboardView));
        }
        #endregion
    }
}
