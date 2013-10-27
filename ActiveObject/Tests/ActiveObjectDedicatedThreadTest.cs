using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace ActiveObject.Tests
{
    [TestFixture]
    public class ActiveObjectDedicatedThreadTest
    {
        private ActiveObjectDedicatedThread m_Active;

        [SetUp]
        public void SetUp()
        {
            m_Active = new ActiveObjectDedicatedThread();
        }

        [Test]
        public void QueueUserWorkItem_WhenSetTempFrom25To40_IsSet()
        {
            // Arrange
            var temperature = 25;
            var waiter = new ManualResetEvent(false);

            // Act
            m_Active.QueueUserWorkItem(x =>
                {
                    temperature = -40;
                    waiter.Set();
                }, null);

            // Assert
            waiter.WaitOne();
            Assert.AreEqual(-40, temperature);
        }


        [Test]
        public void Dispose_WhenQueueUserWorkItemCalled_IsNotExecuted()
        {
            // Arrange
            var temperature = 25;
            var waiter = new ManualResetEvent(false);

            // Act
            m_Active.Dispose();
            m_Active.QueueUserWorkItem(x =>
            {
                temperature = -40;
                waiter.Set();
            }, null);

            // Assert
            Assert.False(waiter.WaitOne(1000), "Work item was executed after dispose");
            Assert.AreEqual(25, temperature);
        }
    }
}
// ReSharper restore InconsistentNaming
