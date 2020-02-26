namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;

    public class SchemaVersionComparer<TSchemaContents> : IComparer<Schema<TSchemaContents>>
    {
        public int Compare(Schema<TSchemaContents> a, Schema<TSchemaContents> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException($"{nameof(a)} cannot be null");
            }

            if (b == null)
            {
                throw new ArgumentNullException($"{nameof(b)} cannot be null");
            }

            return a.Version.CompareTo(b.Version);
        }
    }
}
