namespace ProtoR.Domain.CategoryAggregate
{
    using System.Threading.Tasks;

    public interface ICategoryRepository
    {
         Task<Category> GetById(long id);

         Task<Category> GetByName(string name);

         Task<long> Add(Category category);

         Task Update(Category category);

         Task Delete(long id);
    }
}
