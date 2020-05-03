namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Resource;
    using Apache.Ignite.Core.Services;
    using Autofac;
    using MediatR;
    using ProtoR.Application.Schema;
    using ProtoR.Infrastructure.DataAccess.DependencyInjection;

    [Serializable]
    public class ClusterSingletonService : IService, IClusterSingletonService
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        [InstanceResource]
        private readonly IIgnite ignite = null;
        [NonSerialized]
        private AutoFacPlugin autoFacPlugin;

        public void Cancel(IServiceContext context)
        {
            // No-op
        }

        public void Execute(IServiceContext context)
        {
            // No-op
        }

        public void Init(IServiceContext context)
        {
            this.autoFacPlugin = this.ignite.GetPlugin<AutoFacPlugin>(nameof(AutoFacPluginProvider));
        }

        public async Task<SchemaValidationResultDto> AddSchema(CreateSchemaCommand command)
        {
            var mediator = this.autoFacPlugin.Scope.Resolve<IMediator>();
            await Semaphore.WaitAsync();

            try
            {
                return await mediator.Send(command);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
