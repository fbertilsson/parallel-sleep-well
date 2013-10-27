using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace ActiveObject.NET45.Tests
{
    [TestFixture]
    public class SorterActiveObjectTest
    {
        private SorterActiveObject m_Active;
        private List<string> m_Strings;

        [SetUp]
        public void SetUp()
        {
            m_Active = new SorterActiveObject();
        }

        [Test]
        public void Sort_WhenCBA_SortsToABC()
        {
            // Arrange
            m_Strings = new List<string> { "C", "B", "A" };

            // Act
            var resultEnumerable = m_Active.Sort(m_Strings);

            // Assert
            var result = resultEnumerable.ToList();
            Assert.AreEqual("A", result[0]);
            Assert.AreEqual("B", result[1]);
            Assert.AreEqual("C", result[2]);
        }

        [Test]
        public void SortAsync_WhenCBA_SortsToABC()
        {
            // Arrange
            m_Strings = new List<string> { "charlie", "bravo", "alpha" };

            // Act
            var future = m_Active.SortAsync(m_Strings);

            // Assert
            var result = future.Result.ToList();
            Assert.AreEqual("alpha", result[0]);
            Assert.AreEqual("bravo", result[1]);
            Assert.AreEqual("charlie", result[2]);
        }

        [Test]
        public void SortAsync_WhenClearingListAfterCall_SortsToAlphaBravoCharlie()
        {
            // Arrange
            m_Strings = new List<string> { "charlie", "bravo", "alpha" };

            // Act
            var future = m_Active.SortAsync(m_Strings);
            m_Strings.Clear();

            // Assert
            var result = future.Result.ToList();
            Assert.AreEqual("alpha", result[0]);
            Assert.AreEqual("bravo", result[1]);
            Assert.AreEqual("charlie", result[2]);
        }



        [Test]
        public void SortAsync2_WhenClearingListAfterCall_EmptyResults()
        {
            // Arrange
            m_Strings = new List<string> { "charlie", "bravo", "alpha" };

            // Act
            var future = m_Active.SortAsync2(m_Strings);
            m_Strings.Clear();

            // Assert
            var result = future.Result.ToList();
            Assert.AreEqual("alpha", result[0]);
            Assert.AreEqual("bravo", result[1]);
            Assert.AreEqual("charlie", result[2]);
        }
    }
}
// ReSharper restore InconsistentNaming
