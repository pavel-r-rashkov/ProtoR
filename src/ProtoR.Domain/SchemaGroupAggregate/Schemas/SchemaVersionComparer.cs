namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System.Collections.Generic;

    public class SchemaVersionComparer<TSchemaContents> : IComparer<Schema<TSchemaContents>>
    {
        public int Compare(Schema<TSchemaContents> a, Schema<TSchemaContents> b)
        {
            if (a is null && b is null)
            {
                return 0;
            }

            if (a is null)
            {
                return -1;
            }

            if (b is null)
            {
                return 1;
            }

            return a.Version.CompareTo(b.Version);
        }
    }
}
