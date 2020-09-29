using Serilog.Events;

namespace JOS.HttpDelegatingHandler.Infra.Logging
{
    public class LoggingOverride
    {
        public string Path { get; set; }
        public LogEventLevel Level { get; set; }
    }

}
