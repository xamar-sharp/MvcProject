using Microsoft.Extensions.Configuration;
using System;
namespace MvcProject
{
    public class JsonConfigurationSource:IConfigurationSource
    {
        private string _path;
        public JsonConfigurationSource(string path)
        {
            _path = path;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new JsonConfigurationProvider(_path);
        }
    }
}
