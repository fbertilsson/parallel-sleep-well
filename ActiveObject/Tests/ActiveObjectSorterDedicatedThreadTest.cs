using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace ActiveObject.Tests
{
    [TestFixture]
    public class ActiveObjectSorterDedicatedThreadTest
    {
        private ActiveObjectSorterDedicatedThread m_Active;

        [SetUp]
        public void SetUp()
        {
            m_Active = new ActiveObjectSorterDedicatedThread();
        }

        [Test]
        public void Sort_WhenStringsCleared_CallbackAlphaIsFirst()
        {
            // Arrange
            var waiter = new ManualResetEvent(false);
            var strings = new List<string> {"charlie", "bravo", "alpha"};
            List<string> sortedStrings = null;

            // Act
            m_Active.Sort(strings, 
                result =>
                    {
                        sortedStrings = result.ToList();
                        waiter.Set();
                    });
            strings.Clear();

            // Assert
            waiter.WaitOne();
            Assert.AreEqual("alpha", sortedStrings[0]);
        }


        [Test]
        public void Dispose_WhenQueueUserWorkItemCalled_IsNotExecuted()
        {
            // Arrange
            var temperature = 25;
            var waiter = new ManualResetEvent(false);

            // Act
            m_Active.Dispose();
            m_Active.Sort(new [] {"a", "b"}, r => 
            {
                temperature = -40;
                waiter.Set();
            });

            // Assert
            Assert.False(waiter.WaitOne(1000), "Work item was executed after dispose");
            Assert.AreEqual(25, temperature);
        }

        [Test]
        public void StartTheCpuFan()
        {
            // Arrange
            var sorters = new List<ActiveObjectSorterDedicatedThread>();
            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                sorters.Add(new ActiveObjectSorterDedicatedThread());
            }

            var toSort = new List<string>();
            var random = new Random(); 
            for (var i = 0; i < 500000; i++)
            {
                toSort.Add(random.NextDouble().ToString());
            }

            // Act
            const int nWorkItems = 10;
            int nDone = 0;
            foreach (var sorter in sorters)
            {
                for (var i = 0; i < nWorkItems; i++)
                {
                    sorter.Sort(toSort, x =>
                        {
                            lock (this)
                            {

                                Interlocked.Increment(ref nDone);
                                Monitor.PulseAll(this);
                            }
                        });
                }
            }

            // Assert
            int nTotalWorkItems = nWorkItems * sorters.Count;
            while (nDone < nTotalWorkItems)
            {
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
            Assert.AreEqual(nTotalWorkItems, nDone);
        }

    }
}
// ReSharper restore InconsistentNaming
