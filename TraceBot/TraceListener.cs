using System.Threading.Tasks;

namespace TraceBot
{
    internal class TraceListener : System.Diagnostics.TraceListener
    {
        static BlockEmitter<string> emitter = new BlockEmitter<string>();

        public override void Write(string message)
        {
            emitter.Emit(message);
        }

        public override void WriteLine(string message)
        {
            emitter.Emit(message);
        }

        public static Task<string> ReceiveAsync()
        {
            return emitter.OnAsync();
        }
    }
}
