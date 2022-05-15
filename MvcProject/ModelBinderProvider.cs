using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
namespace MvcProject
{
    public class ModelBinderProvider:IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext ctx)
        {
            return ctx.Metadata.ModelType == typeof(DateTime) ? new ModelBinder() : new SimpleTypeModelBinder(ctx.Metadata.ModelType, new LoggerFactory());
        }
    }
}
