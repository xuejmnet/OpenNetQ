using Microsoft.Extensions.DependencyInjection;

namespace RocketNetQ.Logging.FileLogging.File
{
    public static class FileLoggerFactoryExtensions
    {
        // public static IServiceCollection AddFileLoggerConfiguration(this IServiceCollection services)
        // {
        //     services.AddSingleton<IConfigureOptions<FileLoggerOptions>, FileLoggerOptionsSetup>();
        //     services.AddSingleton<IOptionsChangeTokenSource<FileLoggerOptions>, LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>();
        //     return services;
        // }
        // /// <summary>
        // /// Adds a file logger named 'File' to the factory.
        // /// </summary>
        // /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        // public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        // {
        //     //builder.AddConfiguration();
        //
        //     builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        //     builder.Services.AddFileLoggerConfiguration();
        //     return builder;
        // }
        //
        //
        // /// <summary>
        // /// Adds a file logger named 'File' to the factory.
        // /// </summary>
        // /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        // /// <param name="configure"></param>
        // public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        // {
        //     if (configure == null)
        //     {
        //         throw new ArgumentNullException(nameof(configure));
        //     }
        //
        //     builder.AddFile();
        //     builder.Services.Configure(configure);
        //     return builder;
        // }

        public static IServiceCollection AddInternalFileLog(this IServiceCollection services, Action<FileLoggerOptions> configure)
        {
            services.Configure(configure);
            return services;
        }
    }
}
