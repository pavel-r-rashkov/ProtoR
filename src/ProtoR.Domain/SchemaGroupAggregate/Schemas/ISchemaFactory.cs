namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    public interface ISchemaFactory<TSchema, TSchemaContents>
        where TSchema : Schema<TSchemaContents>
    {
        TSchema CreateNew(Version version, string contents);
    }
}
