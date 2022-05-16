using System;
using System.Reflection;
using System.Text;
namespace MvcProject
{
    public class ConfigurationObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string FormatInfo()
        {
            StringBuilder builder = new StringBuilder(100);
            foreach (var field in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                builder.Append(string.Concat(field.Name, "=", field.GetGetMethod().Invoke(this,new object[] { }).ToString(), ";"));
            }
            return builder.ToString();
        }
    }
}
