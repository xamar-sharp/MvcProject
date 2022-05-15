using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Text;
namespace MvcProject
{
    [HtmlTargetElement("p",TagStructure=TagStructure.NormalOrSelfClosing,ParentTag ="div",Attributes ="CreateLibmanConfiguration")]
    public class FillerTagHelper:TagHelper
    {
        public BitArray Filters { get; set; }
        public Func<string>[] InfoMapping;
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext Context { get; set; }
        public FillerTagHelper()
        {
            InfoMapping = new Func<string>[5];
            InfoMapping[0] = () =>
            {
                StringBuilder builder = new StringBuilder(128);
                builder.AppendLine($"{AppDomain.CurrentDomain.FriendlyName}");
                builder.AppendLine($"{JsonConvert.SerializeObject(AppDomain.CurrentDomain.SetupInformation)}");
                return builder.ToString();
            };
            InfoMapping[1] = () =>
            {
                StringBuilder builder = new StringBuilder(128);
                builder.AppendLine($"{Assembly.GetExecutingAssembly().Location}");
                builder.AppendLine($"{Assembly.GetExecutingAssembly().GetTypes().Length}");
                builder.AppendLine($"{JsonConvert.SerializeObject(Assembly.GetExecutingAssembly().GetName())}");
                return builder.ToString();
            };
            InfoMapping[2] = () =>
            {
                StringBuilder builder = new StringBuilder(128);
                builder.AppendLine($"{ThreadPool.CompletedWorkItemCount};{ThreadPool.PendingWorkItemCount};{ThreadPool.ThreadCount}");
                ThreadPool.GetAvailableThreads(out int worker, out int io);
                builder.AppendLine($"{worker} Workers+{io} IO");
                return builder.ToString();
            };
        }
        public override async Task ProcessAsync(TagHelperContext ctx,TagHelperOutput output)
        {
            if(Filters is null)
            {
                output.SuppressOutput();
                return;
            }
            if(ctx.AllAttributes["CreateLibmanConfiguration"]?.Value as string == "true".ToLowerInvariant())
            {
                using(var writer = File.CreateText(Path.Combine(Environment.CurrentDirectory, "libman.json")))
                {
                    using(JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        await jsonWriter.WriteStartObjectAsync();
                        await jsonWriter.WritePropertyNameAsync("version");
                        await jsonWriter.WriteValueAsync("1.0");
                        await jsonWriter.WritePropertyNameAsync("defaultProvider");
                        await jsonWriter.WriteValueAsync("filesystem");
                        await jsonWriter.WritePropertyNameAsync("libraries");
                        await jsonWriter.WriteStartArrayAsync();
                        await jsonWriter.WriteStartObjectAsync();
                        await jsonWriter.WritePropertyNameAsync("library");
                        await jsonWriter.WriteValueAsync("C:\\4f071486a3ec42b1c791e2e05150794b.jpeg");
                        await jsonWriter.WritePropertyNameAsync("destination");
                        await jsonWriter.WriteValueAsync("~/css/4f071486a3ec42b1c791e2e05150794b.jpeg");
                        await jsonWriter.WriteEndObjectAsync();
                        await jsonWriter.WriteEndArrayAsync();
                        await jsonWriter.WriteEndObjectAsync();
                    }
                }
            }
            output.TagName = "<div>";
            output.TagMode = TagMode.StartTagAndEndTag;
            for(int x = 0; x < Filters.Count; x++)
            {
                if (x < InfoMapping.Length && Filters[x] == true)
                {
                    output.Content.AppendHtml($"<p>{InfoMapping[x]()}</p>");
                }
            }
            await Task.CompletedTask;
        }
    }
}
