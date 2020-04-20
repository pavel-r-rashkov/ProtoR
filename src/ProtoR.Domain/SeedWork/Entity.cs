namespace ProtoR.Domain.SeedWork
{
    using System;

    public abstract class Entity
    {
        public Entity(long id)
        {
            this.Id = id;
        }

        public long Id { get; }
    }
}
