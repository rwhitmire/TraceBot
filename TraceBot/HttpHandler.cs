using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace TraceBot
{
    public class HttpHandler : IHttpHandler
    {
        private static List<WebSocket> sockets = new List<WebSocket>();

        public void ProcessRequest(HttpContext context)
        {
            var n = context.Request.QueryString["n"];

            if (n == "websocket")
            {
                context.AcceptWebSocketRequest(ProcessWSChat);
                return;
            }

            var resourceReader = new ResourceReader();

            if (n == "client.js")
            {
                var js = resourceReader.Read("client.js");
                context.Response.Write(js);
                return;
            }

            var html = resourceReader.Read("index.html");
            context.Response.Write(html);
        }

        public bool IsReusable { get { return false; } }

        private async Task ProcessWSChat(AspNetWebSocketContext context)
        {
            sockets.Add(context.WebSocket);

            while (true)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var message = await TraceListener.ReceiveAsync();

                foreach (var socket in sockets)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
