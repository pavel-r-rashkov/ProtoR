namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.User;
    using ProtoR.Web.Resources.UserResource;

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            this.CreateMap<UserDto, UserReadModel>();

            this.CreateMap<UserPostModel, CreateUserCommand>();

            this.CreateMap<UserPutModel, UpdateUserCommand>();
        }
    }
}
