using Autofac;

namespace Neo.UniversalWallet.Data
{
    public class DataRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ApplicationContext>()
                .As<IApplicationContext>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
