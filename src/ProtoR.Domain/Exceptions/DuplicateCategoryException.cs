namespace ProtoR.Domain.Exceptions
{
    public class DuplicateCategoryException : DomainException
    {
        public DuplicateCategoryException()
        {
        }

        public DuplicateCategoryException(string message)
            : base(message)
        {
        }

        public DuplicateCategoryException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateCategoryException(string message, string categoryName)
            : this(message)
        {
            this.PublicMessage = $"Category with name {categoryName} already exists";
        }
    }
}
