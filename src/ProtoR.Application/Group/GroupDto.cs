namespace ProtoR.Application.Group
{
    using System;

    public class GroupDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
