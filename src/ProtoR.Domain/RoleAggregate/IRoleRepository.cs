namespace ProtoR.Domain.RoleAggregate
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleRepository
    {
        Task<Role> GetById(long id);

        Task<IEnumerable<Role>> GetRoles(IEnumerable<long> ids);

        Task<Role> GetByName(string normalizedName);

        Task<long> Add(Role role);

        Task Delete(long id);

        Task Update(Role role);
    }
}
