namespace ProtoR.Domain.Exceptions
{
    using System;
    using ProtoR.Domain.SeedWork;

    public class EntityNotFoundException<T> : DomainException
        where T : Entity
    {
        private const string DefaultPublicMessage = "Entity was not found";

        public EntityNotFoundException()
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.PublicMessage = DefaultPublicMessage;
        }

        public EntityNotFoundException(object entityIdentifier)
        {
            this.MissingEntityType = typeof(T);
            this.MissingEntityIdentifier = entityIdentifier;
            this.PublicMessage = DefaultPublicMessage;
        }

        public Type MissingEntityType { get; private set; }

        public object MissingEntityIdentifier { get; private set; }
    }
}
