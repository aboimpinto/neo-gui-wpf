using System;
using Autofac;
using GalaSoft.MvvmLight;
using Neo.Gui.ViewModels.Messages;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UI.Core.Wallet.Messages;

namespace Neo.Gui.ViewModels.ScreenViewModels
{
    public class MainViewModel : 
        ViewModelBase,
        IMessageHandler<NavigationMessage>,
        IMessageHandler<InitializeWalletMessage>,
        IMessageHandler<WalletStatusMessage>
    {
        #region Private Fields 
        private static ILifetimeScope _containerLifetimeScope;

        private readonly IMessagePublisher _messagePublisher;

        private object _pageContent;
        private bool _isWalletInitialized;
        private bool _isDashboardLoaded;
        private string _lastBlockSynchronized;
        private string _lastBlockSynchronizedTimeStamp;
        private int _nodeCount;
        private string _synchronizationPercentage;

        #endregion

        #region Public Properties 

        public string LastBlockSynchronized
        {
            get => this._lastBlockSynchronized;
            set
            {
                this._lastBlockSynchronized = value;
                this.RaisePropertyChanged();
            }
        }

        public string LastBlockSynchronizedTimeStamp
        {
            get => this._lastBlockSynchronizedTimeStamp;
            set
            {
                this._lastBlockSynchronizedTimeStamp = value;
                this.RaisePropertyChanged();
            }
        }

        public int NodeCount
        {
            get => this._nodeCount;
            set
            {
                this._nodeCount = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsDashboardLoaded
        {
            get => this._isDashboardLoaded;
            set
            {
                this._isDashboardLoaded = value;
                this.RaisePropertyChanged();
            }
        }

        public string SynchronizationPercentage
        {
            get => this._synchronizationPercentage;
            set
            {
                this._synchronizationPercentage = value;
                this.RaisePropertyChanged();
            }
        }

        public object PageContent
        {
            get => this._pageContent;
            set
            {
                this._pageContent = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Constructor 
        public MainViewModel(
            IMessageSubscriber messageSubscriber,
            IMessagePublisher messagePublisher)
        {
            this._messagePublisher = messagePublisher;

            messageSubscriber.Subscribe(this);
            this.PageContent = LoadView(ViewNames.LoadWalletView);
        }
        #endregion

        #region Public Methods 
        public static void SetLifetimeScope(ILifetimeScope lifetimeScope)
        {
            _containerLifetimeScope = lifetimeScope;
        }
        #endregion

        #region IMessageHandler Implementation 
        public void HandleMessage(NavigationMessage message)
        {
            if (this.PageContent is IView view)
            {
                if (view.DataContext is IUnloadable unloadableView)
                {
                    unloadableView.OnUnload();
                }
            }

            this.PageContent = LoadView(message.DestinationPage);
        }

        public void HandleMessage(InitializeWalletMessage message)
        {
            this._isWalletInitialized = true;
        }

        public void HandleMessage(WalletStatusMessage message)
        {
            if (this._isWalletInitialized)
            {
                if (message.BlockchainStatus.NodeCount == 0)
                {
                    return;
                }

                var lastTimeStamp = DateTime.UtcNow.Subtract(message.BlockchainStatus.TimeSinceLastBlock);
                if (lastTimeStamp == DateTime.MinValue)
                {
                    return;
                }

                if (!this._isDashboardLoaded)
                {
                    this.PageContent = LoadView(ViewNames.DashboardView);
                    this.IsDashboardLoaded = true;
                }

                if (this.LastBlockSynchronized == message.BlockchainStatus.Height.ToString())
                {
                    return;
                }

                // This code only runs once per block.
                this.LastBlockSynchronized = message.BlockchainStatus.Height.ToString();
                this.LastBlockSynchronizedTimeStamp = DateTime.UtcNow.Subtract(message.BlockchainStatus.TimeSinceLastBlock).ToString("yyy-MM-dd HH:mm:ss");
                this.NodeCount = message.BlockchainStatus.NodeCount;

                this.SynchronizationPercentage = (message.BlockchainStatus.Height * 100 / message.BlockchainStatus.HeaderHeight).ToString();

                this._messagePublisher.Publish(new NewBlockReceivedMessage());
            }
        }
        #endregion

        #region Private Methods 
        private static object LoadView(object viewName)
        {
            // TODO [AboimPinto]: To be able to test, a ILifetimeScope wrapper should be created.
            var viewInstance = _containerLifetimeScope.ResolveKeyed<IView>(viewName);
            return viewInstance;
        }
        #endregion
    }
}
