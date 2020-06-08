namespace ProtoR.Application.Schema
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class GetByVersionQueryHandler : IRequestHandler<GetByVersionQuery, SchemaDto>
    {
        private readonly ISchemaDataProvider dataProvider;

        public GetByVersionQueryHandler(ISchemaDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<SchemaDto> Handle(GetByVersionQuery request, CancellationToken cancellationToken)
        {
            SchemaDto schema = request.Version.Equals("latest", StringComparison.InvariantCultureIgnoreCase)
                ? await this.dataProvider.GetLatestVersion(request.GroupName)
                : await this.dataProvider.GetByVersion(request.GroupName, Convert.ToInt32(request.Version, CultureInfo.InvariantCulture));

            if (schema == null)
            {
                throw new EntityNotFoundException<ProtoBufSchema>(request.Version);
            }

            return schema;
        }
    }
}
