using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.IO;
using System;
namespace MvcProject
{
    public static class AnchorHtmlHelper
    {
        public static HtmlString GetAnchorList(this IHtmlHelper helper,KeyValuePair<string,string>[] uriS, string title, bool isCounting = false)
        {
            TagBuilder builder = new TagBuilder("li");
            builder.Attributes["title"] = title;
            for (int x = 0; x < uriS.Length; x++)
            {
                if (Uri.IsWellFormedUriString(uriS[x].Key, UriKind.Absolute))
                {
                    string type = isCounting ? "ol" : "ul";
                    TagBuilder element = new TagBuilder(type);
                    TagBuilder uri = new TagBuilder("a");
                    uri.Attributes["href"] = uriS[x].Key;
                    uri.InnerHtml.SetContent(uriS[x].Value);
                    builder.InnerHtml.AppendHtml(uri);
                    element.InnerHtml.SetHtmlContent(uri);
                    builder.InnerHtml.AppendHtml(element);
                }
            }
            StringWriter writer = new StringWriter();
            builder.InnerHtml.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}
