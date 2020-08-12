namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
    using ProtoR.Application.Common;

    public class FilterBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IEnumerable<Filter>))
            {
                return new BinderTypeModelBinder(typeof(FilterBinder));
            }

            return null;
        }
    }
}
