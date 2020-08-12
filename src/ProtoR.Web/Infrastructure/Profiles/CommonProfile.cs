namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Common;

    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            this.CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
