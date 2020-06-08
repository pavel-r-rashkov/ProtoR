namespace ProtoR.Domain.Exceptions
{
    using System;

    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public string PublicMessage { get; protected set; }
    }
}
