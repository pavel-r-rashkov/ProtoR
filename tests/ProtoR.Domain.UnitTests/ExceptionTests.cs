namespace ProtoR.Domain.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.Exceptions;
    using Xunit;

    public class ExceptionTests
    {
        [Fact]
        public async Task DuplicateCategoryException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateCategoryException>(() => throw new DuplicateCategoryException());
        }

        [Fact]
        public async Task DuplicateCategoryException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateCategoryException>(() => throw new DuplicateCategoryException("Some message"));
        }

        [Fact]
        public async Task DuplicateCategoryException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateCategoryException>(() => throw new DuplicateCategoryException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task DuplicateCategoryException_WithCategory_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateCategoryException>(() => throw new DuplicateCategoryException("Some message", "category name"));
        }

        [Fact]
        public async Task DuplicateGroupException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateGroupException>(() => throw new DuplicateGroupException());
        }

        [Fact]
        public async Task DuplicateGroupException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateGroupException>(() => throw new DuplicateGroupException("Some message"));
        }

        [Fact]
        public async Task DuplicateGroupException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateGroupException>(() => throw new DuplicateGroupException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task DuplicateGroupException_WithGroup_ShouldThrow()
        {
            await Assert.ThrowsAsync<DuplicateGroupException>(() => throw new DuplicateGroupException("Some message", "group name"));
        }

        [Fact]
        public async Task EntityNotFoundException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<Category>>(() => throw new EntityNotFoundException<Category>());
        }

        [Fact]
        public async Task EntityNotFoundException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<Category>>(() => throw new EntityNotFoundException<Category>("Some message"));
        }

        [Fact]
        public async Task EntityNotFoundException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<Category>>(() => throw new EntityNotFoundException<Category>("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task EntityNotFoundException_WithEntityId_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<Category>>(() => throw new EntityNotFoundException<Category>(123));
        }

        [Fact]
        public async Task InaccessibleCategoryException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleCategoryException>(() => throw new InaccessibleCategoryException());
        }

        [Fact]
        public async Task InaccessibleCategoryException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleCategoryException>(() => throw new InaccessibleCategoryException("Some message"));
        }

        [Fact]
        public async Task InaccessibleCategoryException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleCategoryException>(() => throw new InaccessibleCategoryException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task InaccessibleCategoryException_WithPrincipalNameAndCategory_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleCategoryException>(() => throw new InaccessibleCategoryException(123, "Client name"));
        }
    }
}
