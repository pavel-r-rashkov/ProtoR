namespace Web.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Schema;
    using ProtoR.Web.Resources.SchemaResource;

    public class SchemaProfile : Profile
    {
        public SchemaProfile()
        {
            this.CreateMap<SchemaDto, SchemaReadModel>();

            this.CreateMap<SchemaWriteModel, CreateSchemaCommand>();
        }
    }
}
