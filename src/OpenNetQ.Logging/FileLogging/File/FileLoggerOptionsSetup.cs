using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace OpenNetQ.Logging.FileLogging.File
{
    public class FileLoggerOptionsSetup : ConfigureFromConfigurationOptions<FileLoggerOptions>
    {
        public FileLoggerOptionsSetup(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration)
        {

        }
    }
}
