using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UniversalWallet.WPF;

namespace Neo.Gui.ViewModels
{
    public class LoadWalletViewModel : ViewModelBase
    {
        private readonly IMessagePublisher _messagePublisher;
        public RelayCommand UnlockWalletCommand { get; private set; }

        public LoadWalletViewModel(IMessagePublisher messagePublisher)
        {
            this._messagePublisher = messagePublisher;

            this.UnlockWalletCommand = new RelayCommand(this.HandleUnlockWallet);
        }

        private void HandleUnlockWallet()
        {
            this._messagePublisher.Publish(new NavigationMessage("DashboardView"));
        }
    }
}
