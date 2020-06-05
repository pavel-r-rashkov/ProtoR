namespace ProtoR.Domain.UserAggregate
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersInRole(long roleId);

        Task<User> GetByName(string normalizedName);

        Task<User> GetById(long id);

        Task<long> Add(User user);

        Task Delete(long id);

        Task Update(User user);
    }
}
