namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Role;
    using ProtoR.Web.Resources.RoleResource;

    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            this.CreateMap<RoleWriteModel, UpdateRoleCommand>();

            this.CreateMap<RoleWriteModel, CreateRoleCommand>();

            this.CreateMap<RoleDto, RoleReadModel>();
        }
    }
}
