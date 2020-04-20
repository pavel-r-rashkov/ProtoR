namespace ProtoR.Web.Infrastructure.Modules
{
    using System.Reflection;
    using Autofac;
    using MediatR;
    using ProtoR.Application.Group;

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
                .RegisterAssemblyTypes(typeof(GetGroupsQuery).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
        }
    }
}
