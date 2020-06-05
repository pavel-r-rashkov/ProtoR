namespace ProtoR.Domain.RoleAggregate
{
    using System;
    using System.Collections.Generic;
    using ProtoR.Domain.SeedWork;

    public class RoleBinding : ValueObject<RoleBinding>
    {
        public RoleBinding(long roleId, long? userId, long? clientId)
        {
            this.VerifyAssignment(userId, clientId);
            this.RoleId = roleId;
            this.UserId = userId;
            this.ClientId = clientId;
        }

        public long RoleId { get; }

        public long? UserId { get; }

        public long? ClientId { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.RoleId;
            yield return this.UserId;
            yield return this.ClientId;
        }

        private void VerifyAssignment(long? userId, long? clientId)
        {
            if (userId == null && clientId == null)
            {
                throw new ArgumentException($"{nameof(this.UserId)} and {nameof(this.ClientId)} cannot be both null");
            }

            if (userId != null && clientId != null)
            {
                throw new ArgumentException($"{nameof(this.UserId)} and {nameof(this.ClientId)} cannot be both set");
            }
        }
    }
}
