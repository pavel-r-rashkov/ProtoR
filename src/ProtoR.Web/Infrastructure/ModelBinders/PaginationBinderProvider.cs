namespace ProtoR.Web.Infrastructure.ModelBinders
{
    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
    using ProtoR.Application.Common;

    public class PaginationBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Pagination))
            {
                return new BinderTypeModelBinder(typeof(PaginationBinder));
            }

            return null;
        }
    }
}
