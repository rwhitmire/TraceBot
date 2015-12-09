using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace TraceBot
{
    using WebSocketAccept = Action<IDictionary<string, object>, // options
    Func<IDictionary<string, object>, Task>>; // callback

    using WebSocketCloseAsync =
        Func<int /* closeStatus */,
            string /* closeDescription */,
            CancellationToken /* cancel */,
            Task>;

    using WebSocketReceiveAsync =
        Func<ArraySegment<byte> /* data */,
            CancellationToken /* cancel */,
            Task<Tuple<int /* messageType */,
                bool /* endOfMessage */,
                int /* count */>>>;

    using WebSocketSendAsync =
        Func<ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            CancellationToken /* cancel */,
            Task>;

    using WebSocketReceiveResult = Tuple<int, // type
        bool, // end of message?
        int>; // count

    public static class WebSockets
    {
        public static Task Upgrade(IOwinContext context, Func<Task> next)
        {
            if (context.Request.Path.Value != "/ws")
            {
                return next();
            }

            var accept = context.Get<WebSocketAccept>("websocket.Accept");
            var notWebSocketRequest = accept == null;

            if (notWebSocketRequest)
            {
                return next();
            }

            accept(null, WebSocketEcho);

            return Task.FromResult<object>(null);
        }

        private static async Task WebSocketEcho(IDictionary<string, object> websocketContext)
        {
            var sendAsync = (WebSocketSendAsync)websocketContext["websocket.SendAsync"];
            var receiveAsync = (WebSocketReceiveAsync)websocketContext["websocket.ReceiveAsync"];
            var closeAsync = (WebSocketCloseAsync)websocketContext["websocket.CloseAsync"];
            var callCancelled = (CancellationToken)websocketContext["websocket.CallCancelled"];

            var buffer = new byte[1024];
            var received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);

            object status;

            while (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out status) || (int)status == 0)
            {
                await sendAsync(new ArraySegment<byte>(buffer, 0, received.Item3), received.Item1, received.Item2, callCancelled);
                received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
            }

            await closeAsync((int)websocketContext["websocket.ClientCloseStatus"], (string)websocketContext["websocket.ClientCloseDescription"], callCancelled);
        }
    }
}
