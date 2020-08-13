namespace ProtoR.Web.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Common;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using ProtoR.Application.Group;

    public class IgniteHealthCheck : IHealthCheck
    {
        public const string Name = "protor-ignite-check";
        private readonly IGroupDataProvider groupData;

        public IgniteHealthCheck(IGroupDataProvider groupData)
        {
            this.groupData = groupData;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await this.groupData.GetByName(string.Empty);
            }
            catch (IgniteException)
            {
                return HealthCheckResult.Unhealthy(Name);
            }

            return HealthCheckResult.Healthy(Name);
        }
    }
}
