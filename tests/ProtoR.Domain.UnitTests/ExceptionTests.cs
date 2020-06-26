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
        public async Task DuplicateGroupException_WithMessage_ShouldThrow()
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
        public async Task EntityNotFoundException_WithMessage_ShouldThrow()
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
        public async Task InaccessibleGroupException_WithMessage_ShouldThrow()
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

        [Fact]
        public async Task UserException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<UserException>(() => throw new UserException());
        }

        [Fact]
        public async Task UserException_WithMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<UserException>(() => throw new UserException("Some message"));
        }

        [Fact]
        public async Task UserException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<UserException>(() => throw new UserException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task UserException_WithPublicMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<UserException>(() => throw new UserException("Some message", "Public message"));
        }

        [Fact]
        public async Task RoleException_DefaultConstructor_ShouldThrow()
        {
            await Assert.ThrowsAsync<RoleException>(() => throw new RoleException());
        }

        [Fact]
        public async Task RoleException_WithMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<RoleException>(() => throw new RoleException("Some message"));
        }

        [Fact]
        public async Task RoleException_WithInnerException_ShouldThrow()
        {
            await Assert.ThrowsAsync<RoleException>(() => throw new RoleException("Some message", new Exception("Inner exception")));
        }

        [Fact]
        public async Task RoleException_WithPublicMessage_ShouldThrow()
        {
            await Assert.ThrowsAsync<RoleException>(() => throw new RoleException("Some message", "Public message"));
        }
    }
}
