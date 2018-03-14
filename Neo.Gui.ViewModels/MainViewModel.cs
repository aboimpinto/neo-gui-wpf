using Autofac;
using GalaSoft.MvvmLight;
using Neo.UI.Core.Messaging.Interfaces;

namespace Neo.Gui.ViewModels
{
    public class MainViewModel : 
        ViewModelBase,
        IMessageHandler<NavigationMessage>
    {
        private readonly IMessageSubscriber _messageSubscriber;
        private static ILifetimeScope _containerLifetimeScope;

        private object _pageContent;

        public object PageContent
        {
            get => this._pageContent;
            set
            {
                this._pageContent = value;
                this.RaisePropertyChanged();
            }
        }

        public MainViewModel(IMessageSubscriber messageSubscriber)
        {
            this._messageSubscriber = messageSubscriber;

            this._messageSubscriber.Subscribe(this);
            this.PageContent = LoadView("LoadWalletView");
        }

        public static void SetLifetimeScope(ILifetimeScope lifetimeScope)
        {
            _containerLifetimeScope = lifetimeScope;
        }

        private static object LoadView(object viewName)
        {
            var viewInstance = _containerLifetimeScope.ResolveKeyed<IView>(viewName);
            return viewInstance;
        }

        #region IMessageHandler Implementation 
        public void HandleMessage(NavigationMessage message)
        {
            this.PageContent = LoadView(message.DestinationPage);
        }
        #endregion
    }
}
