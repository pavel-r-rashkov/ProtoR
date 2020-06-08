namespace ProtoR.Domain.Exceptions
{
    public class DuplicateGroupException : DomainException
    {
        public DuplicateGroupException()
        {
        }

        public DuplicateGroupException(string message)
            : base(message)
        {
        }

        public DuplicateGroupException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateGroupException(string message, string groupName)
            : this(message)
        {
            this.PublicMessage = $"Group with name {groupName} already exists";
        }
    }
}
