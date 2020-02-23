namespace ProtoR.Domain.SeedWork
{
    using System;

    public abstract class Entity
    {
        public Entity(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; }
    }
}
