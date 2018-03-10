using Autofac;

namespace Neo.UniversalWallet.Model
{
    public class ModelsRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<NodeModel>()
                .As<INodeModel>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
