using System;
using GR.Core.Extensions;
using NUnit.Framework;

namespace GR.Core.Tests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        /// <summary>
        /// Day index
        /// </summary>
        [Test]
        public void DayWeekIndexTest()
        {
            for (var i = 1; i <= 7; i++)
            {
                var date = DateTime.Parse($"10/2{i}/2019");
                Assert.AreEqual(i, date.DayIndex());
            }
        }

        /// <summary>
        /// Start date
        /// </summary>
        [Test]
        public void StartOfDayTest()
        {
            var today = DateTime.Today;
            var startOfDay = today.StartOfDay();
            Assert.AreEqual(startOfDay.Hour, 0);
            Assert.AreEqual(startOfDay.Minute, 0);
        }

        /// <summary>
        /// End of day
        /// </summary>
        [Test]
        public void EndOfDayTest()
        {
            var today = DateTime.Today;
            var endOfDay = today.EndOfDay();
            Assert.AreEqual(endOfDay.Hour, 23);
            Assert.AreEqual(endOfDay.Minute, 59);
        }
    }
}
