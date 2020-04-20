namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISchemaDataProvider
    {
        Task<SchemaDto> GetByVersion(string groupName, int version);

        Task<SchemaDto> GetLatestVersion(string groupName);

        Task<IEnumerable<SchemaDto>> GetGroupSchemas(string groupName);
    }
}
