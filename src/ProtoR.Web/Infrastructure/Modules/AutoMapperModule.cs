namespace ProtoR.Web.Infrastructure.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using AutoMapper;

    public class AutoMapperModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> assembliesToScan;

        public AutoMapperModule(IEnumerable<Assembly> assembliesToScan)
        {
            this.assembliesToScan = assembliesToScan;
        }

        public AutoMapperModule(params Assembly[] assembliesToScan)
            : this((IEnumerable<Assembly>)assembliesToScan)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembliesToScan = this.assembliesToScan as Assembly[] ?? this.assembliesToScan.ToArray();

            var allTypes = assembliesToScan
                .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                .Distinct()
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>),
            };

            IEnumerable<TypeInfo> typeInfoes = openTypes.SelectMany(openType =>
                allTypes.Where(t => t.IsClass && !t.IsAbstract && ImplementsGenericInterface(t.AsType(), openType)));

            foreach (var type in typeInfoes)
            {
                builder.RegisterType(type.AsType()).InstancePerDependency();
            }

            builder.Register<IConfigurationProvider>(ctx => new MapperConfiguration(cfg => cfg.AddMaps(assembliesToScan)));
            builder.Register<IMapper>(ctx => new Mapper(ctx.Resolve<IConfigurationProvider>(), ctx.Resolve)).InstancePerDependency();
        }

        private static bool ImplementsGenericInterface(Type type, Type interfaceType)
        {
            return IsGenericType(type, interfaceType)
                || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => IsGenericType(@interface, interfaceType));
        }

        private static bool IsGenericType(Type type, Type genericType)
        {
            return type.GetTypeInfo().IsGenericType
                && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
