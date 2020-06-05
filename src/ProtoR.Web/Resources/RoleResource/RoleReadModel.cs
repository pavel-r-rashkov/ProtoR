namespace ProtoR.Web.Resources.RoleResource
{
    using System;
    using System.Collections.Generic;

    public class RoleReadModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> Permissions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
