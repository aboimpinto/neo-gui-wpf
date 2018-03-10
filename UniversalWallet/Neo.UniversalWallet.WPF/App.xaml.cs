using Autofac;
using Neo.UniversalWallet.Data;
using Neo.UniversalWallet.Model;
using Neo.UniversalWallet.ViewModels;
using Neo.UniversalWallet.WPF.MarkupExtensions;

namespace Neo.UniversalWallet.WPF
{
    /// <inheritdoc />
    public partial class App 
    {
        public App()
        {
            var containerLifetimeScope = BuildAutofacContainer();

            DataContextBindingExtension.SetLifetimeScope(containerLifetimeScope);
            MainWindowViewModel.SetLifetimeScope(containerLifetimeScope);       // This is not injected because I DON'T WANT TO IMPLEMENT THE ServiceLocator Pattern. Only in the class this is need to load the view.

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private static ILifetimeScope BuildAutofacContainer()
        {
            var autoFacContainerBuilder = new ContainerBuilder();

            autoFacContainerBuilder.RegisterModule<DataRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<ViewsRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<ViewModelsRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<ModelsRegistrationModule>();

            var container = autoFacContainerBuilder.Build();
            var lifetimeScope = container.BeginLifetimeScope();
            return lifetimeScope;
        }
    }
}
