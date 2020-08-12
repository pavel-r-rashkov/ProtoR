namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using ProtoR.Application.Common;

    public class SortOrderBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var orderParam = bindingContext.HttpContext.Request.Query["orderBy"];

            if (!orderParam.Any())
            {
                return Task.CompletedTask;
            }

            var sortOrderTokens = orderParam.SelectMany(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries));

            var sortOrders = sortOrderTokens.Select(s =>
            {
                var sortTokens = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var direction = SortDirection.Ascending;

                if (sortTokens.Length >= 2)
                {
                    direction = sortTokens[1] switch
                    {
                        "asc" => SortDirection.Ascending,
                        "desc" => SortDirection.Descending,
                        _ => throw new ArgumentException("Sort direction must be \"asc\" or \"desc\""),
                    };
                }

                return new SortOrder
                {
                    PropertyName = sortTokens[0],
                    Direction = direction,
                };
            });

            bindingContext.Result = ModelBindingResult.Success(sortOrders);

            return Task.CompletedTask;
        }
    }
}
