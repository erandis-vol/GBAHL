using GBAHL;
using GBAHL.Text;
using GBAHL.Text.Pokemon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GBAHL.Tests
{
    [TestClass]
    public class EncodingTest
    {
        private static Encoding encoding = FireRedEncoding.International;

        private const string VALID_STRING_1 = @"あ... かわいいですね.";
        private const string VALID_STRING_2 = @"HELLO.\nI AM NICE.";

        private const string INVALID_STRING_1 = @"oops [invalid";
        private const string INVALID_STRING_2 = @"oops\";

        [TestMethod]
        public void TestSplit()
        {
            
        }

        /*
        [TestMethod]
        public void TestGetByteCount()
        {
            var c1 = encoding.GetByteCount(VALID_STRING_1);
            Assert.AreEqual(c1, 13);

            var c2 = encoding.GetByteCount(VALID_STRING_2);
            Assert.AreEqual(c2, 17);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestGetByteCount_Invalid()
        {
            encoding.GetByteCount(INVALID_STRING_1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestGetByteCount_Invalid2()
        {
            encoding.GetByteCount(INVALID_STRING_2);
        }
        */
    }
}
