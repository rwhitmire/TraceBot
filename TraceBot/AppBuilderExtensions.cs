using Owin;

namespace TraceBot
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseTraceBot(this IAppBuilder builder)
        {
            builder.Use(WebSockets.Upgrade);
            return builder;
        }
    }
}
