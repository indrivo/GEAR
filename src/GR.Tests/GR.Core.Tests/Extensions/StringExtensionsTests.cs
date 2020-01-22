using System.Text;
using NUnit.Framework;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;

namespace GR.Core.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        /// <summary>
        /// Split string by delimiter
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void SplitTest()
        {
            const string delimiter = "|";
            const string str1 = "test split ";
            const string str2 = "of c#";
            const string str3 = str1 + delimiter + str2;
            var split = StringExtensions.Split(str3, delimiter);
            Assert.AreEqual(split[0], str1);
            Assert.AreEqual(split[1], str2);
        }

        /// <summary>
        /// First char to upper
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void FirstCharToUpperTest()
        {
            Assert.AreEqual("test".FirstCharToUpper(), "Test");
            Assert.AreEqual(string.Empty.FirstCharToUpper(), string.Empty);
        }

        /// <summary>
        /// Is null or empty
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void IsNullOrEmptyTest()
        {
            Assert.IsTrue(string.Empty.IsNullOrEmpty());
            Assert.IsTrue(((string)null).IsNullOrEmpty());
            Assert.IsFalse("test".IsNullOrEmpty());
        }


        /// <summary>
        /// Check valid email
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void IsValidEmailTest()
        {
            const string mail1 = "test@test.com";
            const string mail2 = "test@.com";
            Assert.IsTrue(mail1.IsValidEmail());
            Assert.IsFalse(mail2.IsValidEmail());
        }

        /// <summary>
        /// Strip html
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void StripHtmlTest()
        {
            const string htmlStr = "<div><p>Test</p></div>";
            Assert.AreEqual(htmlStr.StripHtml().Trim(), "Test");
        }

        /// <summary>
        /// Truncate text
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void TruncateTest()
        {
            const string str = "Test for truncate text";
            const string truncatedStr = "Tes...";
            Assert.AreEqual(str.Truncate(6), truncatedStr);
        }

        /// <summary>
        /// Is valid url
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void IsValidUrlTest()
        {
            const string str = "http://google.com";
            const string str1 = "http:/google.com";
            Assert.IsTrue(str.IsValidUrl());
            Assert.IsFalse(str1.IsValidUrl());
        }

        /// <summary>
        /// Is valid ip address
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void IsValidIpAddressTest()
        {
            const string ipStr = "192.168.1.1";
            const string ipStr1 = "192.168.1";

            Assert.IsTrue(ipStr.IsValidIpAddress());
            Assert.IsFalse(ipStr1.IsValidIpAddress());
        }

        /// <summary>
        /// To bytes
        /// </summary>
        [Test]
        [Author(Authors.LUPEI_NICOLAE)]
        public void ToBytesTest()
        {
            const string str = "test";
            Assert.AreEqual(str.ToBytes(), Encoding.ASCII.GetBytes(str));
        }
    }
}
