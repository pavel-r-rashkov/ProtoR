namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    public interface ISchemaFactory<TSchemaContents>
    {
        Schema<TSchemaContents> CreateNew(Version version, string contents);
    }
}
