namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using MediatR;

    public class UpdateUserCommand : IRequest
    {
        public long Id { get; set; }

        public bool IsActive { get; set; }

        public string NewPassword { get; set; }

        public string OldPassword { get; set; }

        public IEnumerable<long> Roles { get; set; }

        public IEnumerable<string> GroupRestrictions { get; set; }
    }
}
