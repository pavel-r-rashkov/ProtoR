namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
    using ProtoR.Application.Common;

    public class SortOrderBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IEnumerable<SortOrder>))
            {
                return new BinderTypeModelBinder(typeof(SortOrderBinder));
            }

            return null;
        }
    }
}
