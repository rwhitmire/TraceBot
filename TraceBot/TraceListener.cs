using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace TraceBot
{
    public class TraceListener : System.Diagnostics.TraceListener
    {
        static BlockEmitter<TraceMessage> emitter = new BlockEmitter<TraceMessage>();

        public override void Write(string message)
        {
            emitter.Emit(BuildMessage(message));
        }

        public override void WriteLine(string message)
        {
            emitter.Emit(BuildMessage(message));
        }

        public static Task<TraceMessage> ReceiveAsync()
        {
            return emitter.OnAsync();
        }

        private TraceMessage BuildMessage(string message)
        {
            return new TraceMessage
            {
                Message = message,
                TraceType = Environment.StackTrace.Contains("Trace.TraceError") ? TraceType.Error : TraceType.Information
            };
            
        }



    }
}
