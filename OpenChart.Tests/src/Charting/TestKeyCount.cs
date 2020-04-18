using NUnit.Framework;
using OpenChart.Charting;
using System;

namespace OpenChart.Tests.Charting
{
    public class TestKeyCount
    {
        [TestCase(-1)]
        [TestCase(0)]
        public void Test_LTEZeroKeyCount(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new KeyCount(value));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Test_GTZeroKeyCount(int value)
        {
            Assert.DoesNotThrow(() => new KeyCount(value));
        }

        [Test]
        public void Test_OnKeyCountChanged()
        {
            var keyCount = new KeyCount(1);
            var calls = 0;

            keyCount.KeyCountChanged += delegate { calls++; };

            keyCount.Value = keyCount.Value;
            Assert.AreEqual(0, calls);

            keyCount.Value++;
            Assert.AreEqual(1, calls);
        }
    }
}