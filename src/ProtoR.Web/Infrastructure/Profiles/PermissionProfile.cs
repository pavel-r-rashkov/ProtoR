namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Permission;
    using ProtoR.Web.Resources.PermissionResource;

    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            this.CreateMap<PermissionDto, PermissionReadModel>();
        }
    }
}
