namespace ProtoR.Domain.Exceptions
{
    using System;

    public abstract class InvalidSchemaException<TSchemaValidationError> : DomainException
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
