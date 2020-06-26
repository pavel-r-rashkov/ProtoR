namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using MediatR;

    public class UpdateRoleCommand : IRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> Permissions { get; set; }
    }
}
