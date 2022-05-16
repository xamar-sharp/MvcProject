using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace MvcProject
{
    public class JsonConfigurationProvider:ConfigurationProvider
    {
        private string _path;
        public JsonConfigurationProvider(string path)
        {
            _path = path;
        }
        public override void Load()
        {
            base.Load();
            ICollection<KeyValuePair<string, string>> collection = new HashSet<KeyValuePair<string, string>>(8, new KeysEqualityComparer());
            string lastKey = null;
            bool isValue = false;
            bool isKey = false;
            using (var reader = File.OpenText($"C:\\{Path.GetFileNameWithoutExtension(_path)}.json"))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    while (jsonReader.Read())
                    {
                        if (isKey)
                        {
                            lastKey = jsonReader.Value.ToString();
                            isKey = false;
                        }
                        if (isValue)
                        {
                            collection.Add(new KeyValuePair<string, string>(lastKey, jsonReader.Value.ToString()));
                            isValue = false;
                        }
                        if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value.ToString() == "key")
                        {
                            isKey = true;
                        }
                        if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value.ToString() == "value")
                        {
                            isValue = true;
                        }
                    }
                }
            }
            Data = new Dictionary<string, string>(collection);
        }
    }
    public struct KeysEqualityComparer : IEqualityComparer<KeyValuePair<string, string>>
    {
        public bool Equals(KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2)
        {
            return pair1.Key == pair2.Key;
        }
        public int GetHashCode(KeyValuePair<string, string> pair)
        {
            return pair.Key.GetHashCode();
        }
    }
}
