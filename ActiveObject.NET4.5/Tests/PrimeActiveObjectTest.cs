using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ActiveObject.NET45.Tests
{
    [TestFixture]
    public class PrimeActiveObjectTest
    {
        private PrimeActiveObject m_Active;

        [SetUp]
        public void SetUp()
        {
            m_Active = new PrimeActiveObject();
        }

        [Test]
        public void GetPrimesUpTo_When0_ReturnsEmptyList()
        {
            Assert.AreEqual(0, m_Active.CalculateAsync(0).Result.Count());
        }

        [Test]
        public void GetPrimesUpTo_When2_Returns2()
        {
            var result = m_Active.CalculateAsync(2).Result;
            Assert.AreEqual(2, result.First());
        }

        [Test]
        public void GetPrimesUpTo_When3_Returns23()
        {
            var result = m_Active.CalculateAsync(3).Result.ToList();
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(3, result[1]);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void WhenDoesTheTaskStartWorking()
        {
            var future = m_Active.CalculateAsync(98000);
            System.Threading.Thread.Sleep(5000);
            var result = future.Result;
            var resultList = result.ToList();
            Assert.AreEqual(9418, resultList.Count);
            Assert.AreEqual(2, resultList[0]);
            Assert.AreEqual(3, resultList[1]);
        }

        [Test]
        public void ManyTasks()
        {
            // Arrange
            var tasks = new List<Task<IEnumerable<int>>>();
            
            // Act

            // Make several calculations
            for (int i = 0; i < 8; i++)
            {
                tasks.Add(m_Active.CalculateAsync(98000));
            }
            
            // Assert
            var result = tasks[0].Result;
            var nResults = result.Count();
            Assert.Greater(nResults, 9000);
            Assert.AreEqual(nResults, tasks[1].Result.Count());
            Assert.AreEqual(nResults, tasks[2].Result.Count());
            Assert.AreEqual(nResults, tasks[3].Result.Count());
            Assert.AreEqual(nResults, tasks[4].Result.Count());
            Assert.AreEqual(nResults, tasks[5].Result.Count());
            Assert.AreEqual(nResults, tasks[6].Result.Count());
            Assert.AreEqual(nResults, tasks[7].Result.Count());
        }

    }
}
