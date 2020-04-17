namespace ProtoR.Web.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Group;
    using ProtoR.Web.Resources.GroupResource;

    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            this.CreateMap<GroupDto, GroupReadModel>();

            this.CreateMap<GroupDto, GroupReadModel>();

            this.CreateMap<GroupWriteModel, CreateGroupCommand>();
        }
    }
}
