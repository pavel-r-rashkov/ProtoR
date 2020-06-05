namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserDataProvider
    {
         Task<UserDto> GetById(long id);

         Task<IEnumerable<UserDto>> GetUsers();
    }
}
