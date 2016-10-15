using Akka.Event;
using Akka.Logger.Serilog;
using System;

namespace Akka.Actor
{
    public static class SerilogUtils
    {
        public static void Log(this IActorContext context, Action<ILoggingAdapter> run)
        {
            var log = context.GetLogger(new SerilogLogMessageFormatter());
            run(log);
        }
    }
}
