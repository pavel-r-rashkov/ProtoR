namespace ProtoR.Web.Infrastructure.Modules
{
    using Autofac;
    using ProtoR.Application;
    using ProtoR.Web.Infrastructure;

    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<UserProvider>()
                .As<IUserProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
