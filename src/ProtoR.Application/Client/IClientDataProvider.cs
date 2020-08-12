namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProtoR.Application.Common;

    public interface IClientDataProvider
    {
        Task<ClientDto> GetById(long id);

        Task<PagedResult<ClientDto>> GetClients(
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination);

        Task<IEnumerable<string>> GetOrigins();
    }
}
