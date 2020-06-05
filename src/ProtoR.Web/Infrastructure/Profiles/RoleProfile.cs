namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Role;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Web.Resources.RoleResource;

    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            this.CreateMap<RoleWriteModel, Role>();

            this.CreateMap<RoleDto, RoleReadModel>();
        }
    }
}
