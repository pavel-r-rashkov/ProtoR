namespace ProtoR.Application.Category
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryDataProvider
    {
         Task<CategoryDto> GetById(long id);

         Task<IEnumerable<CategoryDto>> GetCategories();
    }
}
