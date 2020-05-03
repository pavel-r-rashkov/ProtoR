namespace ProtoR.Infrastructure.DataAccess
{
    using System.Threading.Tasks;
    using ProtoR.Application.Schema;

    public interface IClusterSingletonService
    {
         Task<SchemaValidationResultDto> AddSchema(CreateSchemaCommand command);
    }
}
