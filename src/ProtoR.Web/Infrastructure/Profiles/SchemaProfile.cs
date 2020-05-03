namespace ProtoR.Web.Infrastructure.Profiles
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

            this.CreateMap<SchemaWriteModel, ValidateSchemaCommand>();
        }
    }
}
