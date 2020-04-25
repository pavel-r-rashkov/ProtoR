namespace ProtoR.Infrastructure.DataAccess
{
    using System.Threading.Tasks;
    using ProtoR.Application.Schema;

    public interface IClusterSingletonService
    {
         Task<CreateSchemaCommandResult> AddSchema(CreateSchemaCommand command);
    }
}
