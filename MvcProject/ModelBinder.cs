using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
namespace MvcProject
{
    public class ModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext ctx)
        {
            ValueProviderResult dateResult = ctx.ValueProvider.GetValue("date");
            ValueProviderResult timeResult = ctx.ValueProvider.GetValue("time");
            if (dateResult == ValueProviderResult.None && timeResult == ValueProviderResult.None)
            {
                ctx.Result = ModelBindingResult.Failed();
            }
            else
            {
                if (DateTime.TryParse(dateResult.FirstValue, out DateTime date) && TimeSpan.TryParse(timeResult.FirstValue, out TimeSpan time))
                {
                    ctx.Result = ModelBindingResult.Success(new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Milliseconds));
                }
            }
            await Task.Yield();
        }
    }
}
