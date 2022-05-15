using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.IO;
using System;
namespace MvcProject
{
    public class HtmlView:IView
    {
        public string Path { get; }
        public HtmlView(string path)
        {
            Path = path;
        }
        public async Task RenderAsync(ViewContext ctx)
        {
            await ctx.Writer.WriteLineAsync(await File.ReadAllTextAsync(File.Exists(Path)?Path:"C:\\notfoundpage.html"));
        }
        
    }
}
