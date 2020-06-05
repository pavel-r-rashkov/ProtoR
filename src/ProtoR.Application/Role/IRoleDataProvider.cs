namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleDataProvider
    {
        Task<RoleDto> GetById(long id);

        Task<IEnumerable<RoleDto>> GetRoles();
    }
}
