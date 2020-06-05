namespace ProtoR.Domain.UnitTests.CategoryAggregateTests
{
    using System;
    using AutoFixture;
    using ProtoR.Domain.CategoryAggregate;
    using Xunit;

    public class CategoryTests
    {
        [Fact]
        public void Category_WithNullName_ShouldThrow()
        {
            string name = null;

            Assert.Throws<ArgumentException>(() => new Category(name));
        }

        [Fact]
        public void Category_WithEmptyName_ShouldThrow()
        {
            var name = string.Empty;

            Assert.Throws<ArgumentException>(() => new Category(name));
        }

        [Fact]
        public void Category_ShouldSetProperties()
        {
            var category = new Category("name");

            Assert.NotNull(category.Name);
        }

        [Fact]
        public void Category_WithValidName_ShouldSetName()
        {
            var name = "Test category";

            var category = new Category(1, name);

            Assert.Equal(name, category.Name);
        }
    }
}
