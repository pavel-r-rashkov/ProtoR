namespace ProtoR.Web.Resources.UserResource
{
    using System;
    using System.Collections.Generic;

    public class UserReadModel
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public IEnumerable<long> RoleBindings { get; set; }

        public IEnumerable<long> CategoryBindings { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
