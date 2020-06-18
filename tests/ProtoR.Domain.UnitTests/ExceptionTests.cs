namespace ProtoR.Domain.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.UserAggregate;
    using Xunit;

    public class ExceptionTests
    {
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
            await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => throw new EntityNotFoundException<User>());
        }

        [Fact]
        public async Task EntityNotFoundException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => throw new EntityNotFoundException<User>("Some message"));
        }

        [Fact]
        public async Task EntityNotFoundException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => throw new EntityNotFoundException<User>("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task EntityNotFoundException_WithEntityId_ShouldThrow()
        {
            await Assert.ThrowsAsync<EntityNotFoundException<User>>(() => throw new EntityNotFoundException<User>(123));
        }

        [Fact]
        public async Task InaccessibleGroupException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleGroupException>(() => throw new InaccessibleGroupException());
        }

        [Fact]
        public async Task InaccessibleGroupException_WithMessageMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleGroupException>(() => throw new InaccessibleGroupException("Some message"));
        }

        [Fact]
        public async Task InaccessibleGroupException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleGroupException>(() => throw new InaccessibleGroupException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task InaccessibleGroupException_WithPrincipalNameAndCategory_ShouldThrow()
        {
            await Assert.ThrowsAsync<InaccessibleGroupException>(() => throw new InaccessibleGroupException("Group name", "Client name"));
        }
    }
}
