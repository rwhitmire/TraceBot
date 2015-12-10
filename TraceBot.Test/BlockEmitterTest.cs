using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TraceBot.Test
{
    public class BlockEmitterTest
    {
		[Fact]
		public async Task EmitMessageShouldTriggerOnAsync()
        {
            var emitter = new BlockEmitter<string>();

#pragma warning disable CS4014
            Task.Run(() =>
            {
                Thread.Sleep(50);
                emitter.Emit("message");
            });
#pragma warning restore CS4014

            var message = await emitter.OnAsync();
            Assert.Equal("message", message);
        }
    }
}
