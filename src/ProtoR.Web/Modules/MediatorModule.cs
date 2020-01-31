namespace Web.Modules
{
    using System.Reflection;
    using Autofac;
    using MediatR;
    using Web.Features.Test;

    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder
                .RegisterAssemblyTypes(typeof(TestRequest).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
        }
    }
}
