using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
namespace MvcProject
{
    public class HtmlViewEngine:IViewEngine
    {
        public ViewEngineResult GetView(string executingPath,string viewPath,bool isMainPage)
        {
            return ViewEngineResult.NotFound(viewPath, new string[] { executingPath });
        }
        public ViewEngineResult FindView(ActionContext ctx,string viewPath,bool isMainPage)
        {
            string path = Path.Combine(Environment.CurrentDirectory, $"Areas\\Standard\\Views\\{ctx.RouteData.Values["controller"]}\\{ctx.ActionDescriptor.DisplayName}.html");
            if (!string.IsNullOrEmpty(viewPath))
            {
                path = Path.Combine(Environment.CurrentDirectory, $"Areas\\Standard\\Views\\{viewPath}.html");
            }
            if (File.Exists(path))
            {
                return ViewEngineResult.Found(viewPath, new HtmlView(path));
            }
            else
            {
                return ViewEngineResult.NotFound(viewPath, new string[] { path });
            }
        }
    }
}
