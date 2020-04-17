namespace ProtoR.Web.Resources.GroupResource
{
    using System;

    public class GroupReadModel : GroupWriteModel
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
