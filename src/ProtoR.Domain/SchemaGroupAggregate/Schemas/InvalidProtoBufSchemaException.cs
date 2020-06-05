namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf.Reflection;

    public class InvalidProtoBufSchemaException : InvalidSchemaException<Error[]>
    {
        public InvalidProtoBufSchemaException()
        {
        }

        public InvalidProtoBufSchemaException(string message)
            : base(message)
        {
        }

        public InvalidProtoBufSchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidProtoBufSchemaException(IEnumerable<Error> errors)
            : this("Invalid Protocol Buffer schema")
        {
            this.Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }
    }
}
