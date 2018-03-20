using System;
using Autofac;
using GalaSoft.MvvmLight;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UI.Core.Wallet.Messages;

namespace Neo.Gui.ViewModels
{
    public class MainViewModel : 
        ViewModelBase,
        IMessageHandler<NavigationMessage>,
        IMessageHandler<InitializeWalletMessage>,
        IMessageHandler<WalletStatusMessage>
    {
        #region Private Fields 
        private static ILifetimeScope _containerLifetimeScope;

        private object _pageContent;
        private bool _isWalletInitialized;
        private bool _isDashboardLoaded;
        private string _lastBlockSynchronized;
        private string _lastBlockSynchronizedTimeStamp;
        private int _nodeCount;

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
        public MainViewModel(IMessageSubscriber messageSubscriber)
        {
            messageSubscriber.Subscribe(this);
            this.PageContent = LoadView("LoadWalletView");
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

                this.LastBlockSynchronized = message.BlockchainStatus.Height.ToString();
                this.LastBlockSynchronizedTimeStamp = DateTime.UtcNow.Subtract(message.BlockchainStatus.TimeSinceLastBlock).ToString("yyy-MM-dd HH:mm:ss");
                this.NodeCount = message.BlockchainStatus.NodeCount;

                if (!this._isDashboardLoaded)
                {
                    this.PageContent = LoadView("DashboardView");
                    this.IsDashboardLoaded = true;
                }
            }
        }
        #endregion

        #region Private Methods 
        private static object LoadView(object viewName)
        {
            var viewInstance = _containerLifetimeScope.ResolveKeyed<IView>(viewName);
            return viewInstance;
        }
        #endregion
    }
}
