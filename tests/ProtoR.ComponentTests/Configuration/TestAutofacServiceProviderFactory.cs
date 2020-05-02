namespace ProtoR.ComponentTests.Configuration
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public class TestAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> overrideRegisteredDependencies;

        public TestAutofacServiceProviderFactory(Action<ContainerBuilder> overrideRegisteredDependencies)
        {
            this.overrideRegisteredDependencies = overrideRegisteredDependencies
                ?? throw new ArgumentNullException(nameof(overrideRegisteredDependencies));
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new ContainerBuilder();
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            this.overrideRegisteredDependencies(containerBuilder);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }
    }
}
