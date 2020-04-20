namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGroupDataProvider
    {
        Task<GroupDto> GetByName(string groupName);

        Task<IEnumerable<GroupDto>> GetGroups();
    }
}
