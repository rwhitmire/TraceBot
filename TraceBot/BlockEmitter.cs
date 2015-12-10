using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TraceBot
{
    public class BlockEmitter<T>
    {
        Queue<Func<T>> queue = new Queue<Func<T>>();
        ManualResetEvent hasNewItems = new ManualResetEvent(false);
        ManualResetEvent terminate = new ManualResetEvent(false);
        ManualResetEvent waiting = new ManualResetEvent(false);

        public void Emit(T obj)
        {
            lock (queue)
            {
                queue.Enqueue(() => { return obj; });
            }

            hasNewItems.Set();
        }

        public Task<T> OnAsync()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    waiting.Set();

                    var i = WaitHandle.WaitAny(new WaitHandle[] { hasNewItems, terminate });
                    var terminateSignaled = i == 1;

                    if (terminateSignaled) return default(T);

                    hasNewItems.Reset();
                    waiting.Reset();

                    Queue<Func<T>> queueCopy;

                    lock (queue)
                    {
                        queueCopy = new Queue<Func<T>>(queue);
                        queue.Clear();
                    }

                    foreach (var func in queueCopy)
                    {
                        return func();
                    }
                }
            });
        }
    }
}
