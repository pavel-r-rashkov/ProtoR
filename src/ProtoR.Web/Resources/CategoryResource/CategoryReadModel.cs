namespace ProtoR.Web.Resources.CategoryResource
{
    using System;

    public class CategoryReadModel : CategoryWriteModel
    {
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
