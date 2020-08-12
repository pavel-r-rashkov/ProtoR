namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProtoR.Application.Common;

    public interface ISchemaDataProvider
    {
        Task<SchemaDto> GetByVersion(string groupName, int version);

        Task<SchemaDto> GetLatestVersion(string groupName);

        Task<PagedResult<SchemaDto>> GetGroupSchemas(
            string groupName,
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination);
    }
}
