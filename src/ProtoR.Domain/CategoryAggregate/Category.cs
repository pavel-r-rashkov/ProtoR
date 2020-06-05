namespace ProtoR.Domain.CategoryAggregate
{
    using System;
    using ProtoR.Domain.SeedWork;

    public class Category : Entity, IAggregateRoot
    {
        public const long DefaultCategoryId = 1;
        private string name;

        public Category(string name)
            : base(default)
        {
            this.Name = name;
        }

        public Category(
            long id,
            string name)
            : base(id)
        {
            this.Name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.Name)} cannot be null or white space");
                }

                this.name = value;
            }
        }

        public static Category CreateDefault()
        {
            return new Category(DefaultCategoryId, "Default");
        }
    }
}
