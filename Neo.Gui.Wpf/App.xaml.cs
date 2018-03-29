using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

using Autofac;
using Neo.Gui.ViewModels;
using Neo.Gui.Wpf.MarkupExtensions;
using Neo.Gui.Wpf.Native;
using Neo.Gui.Wpf.Properties;
using Neo.Gui.Wpf.Screens;
using Neo.Gui.Wpf.Views;

using Neo.UI.Core.Globalization.Resources;
using Neo.UI.Core.Wallet;
using ViewModelsRegistrationModule = Neo.Gui.ViewModels.ViewModelsRegistrationModule;
using WpfProjectViewModelsRegistrationModule = Neo.Gui.Wpf.Views.ViewModelsRegistrationModule;

namespace Neo.Gui.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ILifetimeScope _lifetimeScope;
        
        private App()
        {
            this.InitializeComponent();

            this.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var walletController = this._lifetimeScope.Resolve<IWalletController>();
            walletController.Dispose();

            base.OnExit(e);
        }

        private void Initialize()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            this._lifetimeScope = BuildContainer(false);

            Debug.Assert(this._lifetimeScope != null);

            DataContextBindingExtension.SetLifetimeScope(this._lifetimeScope);
            MainViewModel.SetLifetimeScope(this._lifetimeScope);       // This is not injected because I DON'T WANT TO IMPLEMENT THE ServiceLocator Pattern. Only in the class this is need to load the view.

            InstallRootCertificateIfRequired();

            this.MainWindow = new MainView();
            this.MainWindow.Show();
        }

        private static ILifetimeScope BuildContainer(bool lightMode)
        {
            var autoFacContainerBuilder = new ContainerBuilder();

            WalletRegistrationModule.LightMode = lightMode;
            autoFacContainerBuilder.RegisterModule<WalletRegistrationModule>();

            autoFacContainerBuilder.RegisterModule<NativeServicesRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<ViewsRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<WpfProjectViewModelsRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<ViewModelsRegistrationModule>();
            autoFacContainerBuilder.RegisterModule<DialogsRegistrationModule>();

            var container = autoFacContainerBuilder.Build();
            var lifetimeScope = container.BeginLifetimeScope();

            return lifetimeScope;
        }

        private static void InstallRootCertificateIfRequired()
        {
            // Only install if using a local node
            // TODO Is the root certificate required if connecting to a remote node?
            if (Settings.Default.LightWalletMode) return;

            if (!Settings.Default.InstallCertificate) return;

            // Check if root certificate is already installed
            if (RootCertificate.IsInstalled) return;

            // Confirm with user before trying to install root certificate
            if (MessageBox.Show(Strings.InstallCertificateText, Strings.InstallCertificateCaption,
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes)
            {
                // User has chosen not to install the Onchain root certificate
                Settings.Default.InstallCertificate = false;
                Settings.Default.Save();
                return;
            }

            // Try install root certificate
            var certificateInstalled = RootCertificate.Install();

            if (certificateInstalled) return;

            var runAsAdmin = MessageBox.Show(
                "Onchain root certificate could not be installed! Do you want to try running the application as adminstrator?",
                "Root certificate installation failed!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (runAsAdmin != MessageBoxResult.Yes) return;

            // Try running application as administrator to install root certificate
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = Environment.CurrentDirectory
                });

                // Stop this application instance
                Current.Shutdown();
            }
            catch (Win32Exception)
            {
                // TODO Log exception somewhere
            }
        }

        #region Unhandled exception methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (var fileStream = new FileStream("error.log", FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                PrintErrorLogs(writer, (Exception)e.ExceptionObject);
            }
        }

        private static void PrintErrorLogs(TextWriter writer, Exception ex)
        {
            writer.WriteLine(ex.GetType());
            writer.WriteLine(ex.Message);
            writer.WriteLine(ex.StackTrace);

            // Print inner exceptions if there are any
            if (ex is AggregateException ex2)
            {
                foreach (var innerException in ex2.InnerExceptions)
                {
                    writer.WriteLine();
                    PrintErrorLogs(writer, innerException);
                }
            }
            else if (ex.InnerException != null)
            {
                writer.WriteLine();
                PrintErrorLogs(writer, ex.InnerException);
            }
        }
        #endregion
    }
}