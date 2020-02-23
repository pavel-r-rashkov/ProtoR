namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;

    public abstract class InvalidSchemaException<TSchemaValidationError> : Exception
    {
        public InvalidSchemaException()
        {
        }

        public InvalidSchemaException(string message)
            : base(message)
        {
        }

        public InvalidSchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
