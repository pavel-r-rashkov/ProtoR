namespace ProtoR.Domain.Exceptions
{
    using System;

    public class RoleException : DomainException
    {
        public RoleException()
        {
        }

        public RoleException(string message)
            : base(message)
        {
        }

        public RoleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RoleException(string message, string publicMessage)
            : base(message)
        {
            this.PublicMessage = publicMessage;
        }
    }
}
