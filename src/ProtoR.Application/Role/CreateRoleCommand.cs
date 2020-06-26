namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using MediatR;

    public class CreateRoleCommand : IRequest<long>
    {
        public string Name { get; set; }

        public IEnumerable<int> Permissions { get; set; }
    }
}
