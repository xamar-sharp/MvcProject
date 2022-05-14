using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
namespace MvcProject.Components
{
    public class SecureSectionComponent:ViewComponent
    {
        public IViewComponentResult Invoke(string viewName,string html=null)
        {
            if(html is not null)
            {
                return new HtmlContentViewComponentResult(new HtmlString(html));
            }
            else
            {
                return View(viewName);
            }
        }
    }
}
