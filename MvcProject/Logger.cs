using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;
namespace MvcProject
{
    public class Logger:ILogger
    {
        private readonly string _resolvingPath;
        private readonly CancellationTokenSource _source;
        public Logger(string path)
        {
            _resolvingPath = path;
            _source = new CancellationTokenSource();
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return typeof(TState).GetInterface(nameof(System.IDisposable)) is null ? null : state as IDisposable;
        }
        public bool IsEnabled(LogLevel level)
        {
            return level != LogLevel.Debug;
        }
        public async void Log<TState>(LogLevel level,EventId id,TState state,Exception ex,Func<TState,Exception,string> formatter)
        {
            try
            {
                using (TextWriter stream = File.AppendText(_resolvingPath))
                {
                    CancellationToken token = _source.Token;

                    _source.CancelAfter(15000);
                    using (JsonTextWriter writer = new JsonTextWriter(stream))
                    {
                        await writer.WriteStartObjectAsync(token);
                        await writer.WritePropertyNameAsync("LogLevel", token);
                        await writer.WriteValueAsync(level.ToString(), token);
                        await writer.WritePropertyNameAsync("Exception", token);
                        await writer.WriteValueAsync(ex is null ? "none" : string.Concat(ex.Message, ex.StackTrace), token);
                        await writer.WritePropertyNameAsync("OtherInfo", token);
                        await writer.WriteStartArrayAsync(token);
                        await writer.WriteValueAsync(id.ToString(), token);
                        await writer.WriteValueAsync(state.ToString(), token);
                        await writer.WriteValueAsync(formatter(state, ex), token);
                        await writer.WriteEndArrayAsync(token);
                        await writer.WriteEndObjectAsync(token);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
