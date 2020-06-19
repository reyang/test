using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace Microsoft.Extensions.Logging
{
    public static class AwesomeLoggerExtensions
    {
        public static ILoggingBuilder AddAwesome(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AwesomeLoggerProvider>());
            // LoggerProviderOptions.RegisterProviderOptions<AwesomeLoggerOptions, AwesomeLoggerProvider>(builder.Services);
            return builder;
        }
    }
}
