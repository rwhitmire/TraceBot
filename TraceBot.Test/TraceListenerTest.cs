﻿using System.Diagnostics;
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

            var result = await TraceListener.ReceiveAsync();
            Assert.Equal("message", result.Message);
            Assert.Equal(TraceType.Information, result.TraceType);
            Trace.Listeners.RemoveAt(i);
        }

        [Fact]
        public async Task TraceErrorShouldTriggerRecieveAsync()
        {
            var i = Trace.Listeners.Add(new TraceListener());

#pragma warning disable CS4014
            Task.Run(() =>
            {
                Thread.Sleep(50);
                Trace.TraceError("");
            });
#pragma warning restore CS4014

            var task = TraceListener.ReceiveAsync();
            var result = await task;
            Assert.Equal("PROGRAM Error: 0 : ", result.Message);
            Assert.Equal(TraceType.Error, result.TraceType);
            Trace.Listeners.RemoveAt(i);
        }
    }
}
