namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using ProtoR.Application.Common;

    public class FilterBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var filterParam = bindingContext.HttpContext.Request.Query["filter"];

            if (!filterParam.Any())
            {
                return Task.CompletedTask;
            }

            var filterQueryParams = filterParam.SelectMany(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries));

            var filters = filterQueryParams.Select(s =>
            {
                var filterTokens = s.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                var type = filterTokens[1] switch
                {
                    "eq" => FilterType.Equal,
                    "ne" => FilterType.NotEqual,
                    "lt" => FilterType.LessThan,
                    "le" => FilterType.LessThanOrEqual,
                    "gt" => FilterType.GreaterThan,
                    "ge" => FilterType.GreaterThanOrEqual,
                    _ => throw new ArgumentException("Filter type must be one of \"eq\", \"ne\", \"lt\", \"le\", \"gt\", \"ge\""),
                };

                return new Filter
                {
                    PropertyName = filterTokens[0],
                    Type = type,
                    Value = filterTokens[2],
                };
            });

            bindingContext.Result = ModelBindingResult.Success(filters);

            return Task.CompletedTask;
        }
    }
}
