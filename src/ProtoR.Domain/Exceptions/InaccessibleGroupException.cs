namespace ProtoR.Domain.Exceptions
{
    using System;

    public class InaccessibleGroupException : DomainException
    {
        private const string DefaultPublicMessage = "Group is not accessible";

        public InaccessibleGroupException()
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleGroupException(string message)
            : base(message)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleGroupException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleGroupException(string groupName, string principalName)
            : this($"User or Client {principalName} cannot access group {groupName}")
        {
            this.GroupName = groupName;
            this.PrincipalName = principalName;
            this.PublicMessage = DefaultPublicMessage;
        }

        public string GroupName { get; }

        public string PrincipalName { get; }
    }
}
