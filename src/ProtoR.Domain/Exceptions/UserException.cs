namespace ProtoR.Domain.Exceptions
{
    using System;

    public class UserException : DomainException
    {
        public UserException()
        {
        }

        public UserException(string message)
            : base(message)
        {
        }

        public UserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UserException(string message, string publicMessage)
            : base(message)
        {
            this.PublicMessage = publicMessage;
        }
    }
}
