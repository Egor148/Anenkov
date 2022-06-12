using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kurs_valut.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
    public class ValuteTest
    {
        Valute _valute;
        [TestInitialize]
        public void ValuteInitializeTest()
        {
            _valute = new Valute();
            Assert.IsNotNull(_valute);
        }
    }
}
