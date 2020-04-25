namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Transactions;
    using ProtoR.Domain.SeedWork;

    public sealed class IgniteUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ITransaction transaction;

        public IgniteUnitOfWork(IIgniteFactory igniteFactory)
        {
            var transactions = igniteFactory.Instance().GetTransactions();
            this.transaction = transactions.TxStart();
        }

        public void Dispose()
        {
            this.transaction.Dispose();
        }

        public async Task SaveChanges()
        {
            await this.transaction.CommitAsync();
        }
    }
}
