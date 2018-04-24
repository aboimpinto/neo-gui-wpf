using Autofac;
using Neo.Gui.ViewModels;
using Neo.Gui.Wpf.Screens;

namespace Neo.Gui.Wpf.Views
{
    public class ViewsRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<LoadWalletView>()
                .Keyed<IView>("LoadWalletView");

            builder
                .RegisterType<DashboardView>()
                .Keyed<IView>("DashboardView");

            builder
                .RegisterType<ManageAccountsView>()
                .Keyed<IView>(ViewNames.ManageAccountsView);

            base.Load(builder);
        }
    }
}
