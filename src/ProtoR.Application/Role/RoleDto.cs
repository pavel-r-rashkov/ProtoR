namespace ProtoR.Application.Role
{
    using System;
    using System.Collections.Generic;

    public class RoleDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> Permissions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
