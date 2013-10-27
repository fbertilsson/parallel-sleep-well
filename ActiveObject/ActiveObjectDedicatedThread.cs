using System;
using System.Collections.Generic;
using System.Threading;

namespace ActiveObject
{
    /// <summary>
    /// An active object with a dedicated private thread.
    /// Pasted and improved from: 
    /// Ordered Execution With ThreadPool, Stephen Toub
    /// http://msdn.microsoft.com/en-us/magazine/dd419664.aspx
    /// </summary>
    public class ActiveObjectDedicatedThread : IDisposable
    {
        private readonly BlockingQueue<WorkItem> m_WorkItems = new BlockingQueue<WorkItem>();
        private readonly WorkItem m_EndMarker = new WorkItem();

        public ActiveObjectDedicatedThread()
        {
            new Thread(() =>
                {
                    var hasEndMarkerArrived = false;
                    while (!hasEndMarkerArrived)
                    {
                        var workItem = m_WorkItems.Dequeue();
                        if (workItem != m_EndMarker)
                        {
                            workItem.Execute();
                        }
                        else
                        {
                            hasEndMarkerArrived = true;
                        }
                    }
                }) {IsBackground = true}.Start();
        }

        /// <summary>
        /// Enqueues work
        /// </summary>
        /// <param name="callback">The work to run</param>
        /// <param name="state"></param>
        public void QueueUserWorkItem(WaitCallback callback, object state)
        {
            m_WorkItems.Enqueue(new WorkItem { Callback = callback, State = state, Context = ExecutionContext.Capture() });
        }

        /// <summary>
        /// Simple blocking queue
        /// </summary>
        /// <typeparam name="T">The content type of the queue</typeparam>
        private class BlockingQueue<T>
        {
            private readonly Queue<T> m_Queue = new Queue<T>();
            private readonly Semaphore m_Gate = new Semaphore(0, Int32.MaxValue);

            public void Enqueue(T item)
            {
                lock (m_Queue) m_Queue.Enqueue(item);
                m_Gate.Release();
            }

            public T Dequeue()
            {
                m_Gate.WaitOne();
                lock (m_Queue) return m_Queue.Dequeue();
            }
        }

        /// <summary>
        /// An item of work that can be placed in a queue and executed later.
        /// </summary>
        private class WorkItem
        {
            public WaitCallback Callback;
            public object State;
            public ExecutionContext Context;

            private static readonly ContextCallback TheContextCallback = s =>
            {
                var item = (WorkItem)s;
                item.Callback(item.State);
            };

            public void Execute()
            {
                if (Context != null) ExecutionContext.Run(Context, TheContextCallback, this);
                else Callback(State);
            }
        }
        /// <summary>
        /// N.B. Simplistic impl.
        /// </summary>
        public void Dispose()
        {
            m_WorkItems.Enqueue(m_EndMarker);
        }
    }




}
