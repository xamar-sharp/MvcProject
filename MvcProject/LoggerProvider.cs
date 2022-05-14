using Microsoft.Extensions.Logging;
namespace MvcProject
{
    public struct DisposableObject : System.IDisposable
    {
        public void Dispose()
        {
            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
        }
    }
    public class LoggerProvider : ILoggerProvider
    {
        private string _resolvingPath;
        private Logger _logger;
        public LoggerProvider(string path)
        {
            _resolvingPath = path;
            _logger = new Logger(_resolvingPath);
        }
        public void Dispose()
        {
            _logger.BeginScope(new DisposableObject()).Dispose();
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }
    }
}
