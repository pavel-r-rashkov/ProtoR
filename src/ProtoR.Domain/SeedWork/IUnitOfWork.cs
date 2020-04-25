namespace ProtoR.Domain.SeedWork
{
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        Task SaveChanges();
    }
}
