namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using ProtoR.Application.Common;

    public class PaginationBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var pageParam = bindingContext.HttpContext.Request.Query["page"];
            var sizeParam = bindingContext.HttpContext.Request.Query["size"];

            if (!pageParam.Any() || !sizeParam.Any())
            {
                return Task.CompletedTask;
            }

            var page = int.Parse(pageParam.First(), CultureInfo.InvariantCulture);
            var size = int.Parse(sizeParam.First(), CultureInfo.InvariantCulture);
            bindingContext.Result = ModelBindingResult.Success(new Pagination { Page = page, Size = size });

            return Task.CompletedTask;
        }
    }
}
