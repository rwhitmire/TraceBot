using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TraceBot.Test
{
    public class TraceListenerTest
    {
        [Fact]
        public async Task WriteLineShouldTriggerReceiveAsync()
        {
            var i = Trace.Listeners.Add(new TraceListener());

#pragma warning disable CS4014
            Task.Run(() =>
            {
                Thread.Sleep(50);
                Trace.WriteLine("message");
            });
#pragma warning restore CS4014

            var message = await TraceListener.ReceiveAsync();
            Assert.Equal("message", message);
            Trace.Listeners.RemoveAt(i);
        }
    }
}
