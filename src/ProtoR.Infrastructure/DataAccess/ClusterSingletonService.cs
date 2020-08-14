namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Resource;
    using Apache.Ignite.Core.Services;
    using Autofac;
    using MediatR;
    using ProtoR.Application.Schema;
    using ProtoR.Infrastructure.DataAccess.DependencyInjection;
    using Serilog;

    [Serializable]
    public class ClusterSingletonService : IService, IClusterSingletonService
    {
        private const string MutexNameFormat = "PROTOR_ADD_SCHEMA_{0}";
        private readonly int mutexWaitTimeout = 1000 * 30;
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

        public Task<SchemaValidationResultDto> AddSchema(CreateSchemaCommand command)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));
            using var childScope = this.autoFacPlugin.Scope.BeginLifetimeScope();
            var mediator = childScope.Resolve<IMediator>();
            var logger = childScope.Resolve<ILogger>();

            var mutexName = string.Format(CultureInfo.InvariantCulture, MutexNameFormat, command.Name);
            using var mutex = new Mutex(true, mutexName);

            if (!mutex.WaitOne(this.mutexWaitTimeout))
            {
                throw new InvalidOperationException("Add schema mutex cannot be acquired");
            }

            try
            {
                var commandResult = mediator.Send(command);
                commandResult.Wait();

                return commandResult;
            }
            catch (AggregateException ex)
            {
                logger.Error(ex, "Error adding schema from cluster singleton service");
                throw ex.InnerException;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
