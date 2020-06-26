namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using MediatR;

    public class CreateUserCommand : IRequest<long>
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<long> Roles { get; set; }

        public IEnumerable<string> GroupRestrictions { get; set; }
    }
}
