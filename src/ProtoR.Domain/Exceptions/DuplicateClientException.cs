namespace ProtoR.Domain.Exceptions
{
    using System;

    public class DuplicateClientException : DomainException
    {
        public DuplicateClientException()
        {
        }

        public DuplicateClientException(string message)
            : base(message)
        {
        }

        public DuplicateClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateClientException(string message, string clientId)
            : this(message)
        {
            this.PublicMessage = $"Client with ID {clientId} already exists";
        }
    }
}
