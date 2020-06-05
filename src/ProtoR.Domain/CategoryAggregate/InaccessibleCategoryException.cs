namespace ProtoR.Domain.CategoryAggregate
{
    using System;

    public class InaccessibleCategoryException : Exception
    {
        public InaccessibleCategoryException()
        {
        }

        public InaccessibleCategoryException(string message)
            : base(message)
        {
        }

        public InaccessibleCategoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InaccessibleCategoryException(long categoryId, string userName)
            : this($"User {userName ?? "anonymous"} cannot access category {categoryId}")
        {
            this.CategoryId = categoryId;
            this.UserName = userName;
        }

        public long CategoryId { get; }

        public string UserName { get; }
    }
}
