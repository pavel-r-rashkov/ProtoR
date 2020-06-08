namespace ProtoR.Domain.Exceptions
{
    using System;

    public class InaccessibleCategoryException : DomainException
    {
        private const string DefaultPublicMessage = "Category is not accessible";

        public InaccessibleCategoryException()
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleCategoryException(string message)
            : base(message)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleCategoryException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public InaccessibleCategoryException(long categoryId, string name)
            : this($"User or Client {name ?? "anonymous"} cannot access category {categoryId}")
        {
            this.CategoryId = categoryId;
            this.PrincipalName = name;
            this.PublicMessage = DefaultPublicMessage;
        }

        public long CategoryId { get; }

        public string PrincipalName { get; }
    }
}
