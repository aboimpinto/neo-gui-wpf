using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UI.Core.Services.Interfaces;
using Neo.UI.Core.Wallet;
using Neo.UI.Core.Wallet.Initialization;
using Neo.UI.Core.Wallet.Messages;

namespace Neo.Gui.ViewModels
{
    public class LoadWalletViewModel : 
        ViewModelBase, 
        ILoadable,
        IUnloadable,
        IMessageHandler<WalletStatusMessage>
    {
        #region Private Fields 
        private readonly IWalletController _walletController;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageSubscriber _messageSubscriber;
        private readonly IFileManager _fileManager;
        private readonly ISettingsManager _settingsManager;

        private string _walletPath;
        private string _walletPassword;
        private string _connectionStatus;

        #endregion

        #region Public Properties

        public string WalletPath
        {
            get => this._walletPath;
            set
            {
                this._walletPath = value;
                this.RaisePropertyChanged();
            }
        }

        public string WalletPassword
        {
            get => this._walletPassword;
            set
            {
                this._walletPassword = value;
                this.RaisePropertyChanged();
            }
        }

        public string ConnectionStatus
        {
            get => this._connectionStatus;
            set
            {
                this._connectionStatus = value;
                this.RaisePropertyChanged();
            }
        }

        public RelayCommand UnlockWalletCommand { get; private set; }
        #endregion

        #region Constructor 
        public LoadWalletViewModel(
            IWalletController walletController,
            IMessagePublisher messagePublisher,
            IMessageSubscriber messageSubscriber,
            IFileManager fileManager,
            ISettingsManager settingsManager)
        {
            this._walletController = walletController;
            this._messagePublisher = messagePublisher;
            this._messageSubscriber = messageSubscriber;
            this._fileManager = fileManager;
            this._settingsManager = settingsManager;

            this.UnlockWalletCommand = new RelayCommand(this.HandleUnlockWallet);
        }
        #endregion

        #region ILoadable Implementation 
        public void OnLoad()
        {
            this._messageSubscriber.Subscribe(this);

            var lastWalletPath = this._settingsManager.LastWalletPath;

            if (string.IsNullOrEmpty(lastWalletPath)) return;

            if (this._fileManager.FileExists(lastWalletPath))
            {
                this.WalletPath = lastWalletPath;
            }

            this.ConnectionStatus = string.Empty;
        }
        #endregion

        #region IMessageHandler Implementation 
        public void HandleMessage(WalletStatusMessage message)
        {
            this.ConnectionStatus = "Connected to NEO Network";

            if (!this._walletController.WalletIsOpen) return;

            this._messagePublisher.Publish(new InitializeWalletMessage());
        }
        #endregion

        #region IUnloadable Implementation 
        public void OnUnload()
        {
            this._messageSubscriber.Unsubscribe(this);
        }
        #endregion

        #region Private Methods 
        private void HandleUnlockWallet()
        {
            this.ConnectionStatus = "Initializing Wallet";
            var initializationParameters = new FullWalletInitializationParameters(20333, 10333, "ChainPrivateNet", "Certs");        // TODO [AboimPinto]: Need to get this values from the Settings
            this._walletController.Initialize(initializationParameters);

            this.ConnectionStatus = "Load NEP5 Script Hashes";
            this._walletController.SetNEP5WatchScriptHashes(new string[] { });

            this.ConnectionStatus = "Opening the Wallet";
            this._walletController.OpenWallet(this.WalletPath, this.WalletPassword);

            this.ConnectionStatus = "Save settings";
            this._settingsManager.LastWalletPath = this.WalletPath;
            this._settingsManager.Save();
        }
        #endregion
    }
}
