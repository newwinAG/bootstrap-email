using bootstrap_email;
using NUnit.Framework;

namespace BootstrapEmailTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// TODO Fix inlined Files for Unittests
        /// </summary>
        [Test]
        public void Alert1()
        {
            var tmpFile = UnitTestHelper.LoadFile("alert.html", "preinlined");
            var tmpResult = BootstrapEmail.Parse(tmpFile);

            var tmpExpectedResult = UnitTestHelper.LoadFile("alert.html", "inlined");

            var tmpCompareResult = UnitTestHelper.CompareHtmlFiles(tmpResult, tmpExpectedResult);

            Assert.AreEqual(tmpCompareResult, true);
        }

        /// <summary>
        /// TODO Fix inlined Files for Unittests
        /// </summary>
        [Test]
        public void Hr1()
        {
            var tmpFile = UnitTestHelper.LoadFile("hr.html", "preinlined");
            var tmpResult = BootstrapEmail.Parse(tmpFile);

            var tmpExpectedResult = UnitTestHelper.LoadFile("hr.html", "inlined");

            var tmpCompareResult = UnitTestHelper.CompareHtmlFiles(tmpResult, tmpExpectedResult);

            Assert.AreEqual(tmpCompareResult, true);
        }
    }
}