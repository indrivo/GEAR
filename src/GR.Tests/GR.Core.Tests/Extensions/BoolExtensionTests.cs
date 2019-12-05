using GR.Core.Extensions;
using NUnit.Framework;

namespace GR.Core.Tests.Extensions
{
    [TestFixture]
    public class BoolExtensionTests
    {
        /// <summary>
        /// Negate tests
        /// </summary>
        [Test]
        [Author("Lupei Nicolae")]
        public void NegateTest()
        {
            Assert.IsTrue(false.Negate());
            Assert.IsFalse(true.Negate());
        }
    }
}
