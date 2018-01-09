using GBAHL.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;

namespace GBAHL.Tests
{
    [TestClass]
    public class PaletteTest
    {
        private static Palette Grayscale = CreateGrayscale(256);

        private static Palette CreateGrayscale(int length)
        {
            var step = 256 / length;

            var pltt = new Palette(Color.Black, length);
            for (int i = 0; i < length; i++)
                pltt[i] = Color.FromArgb(i * step, i * step, i * step);

            return pltt;
        }

        private static bool AreEqual(Palette a, Palette b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        // NOTE: make sure to run TestSave() first to generate saved files

        [TestMethod]
        public void TestLoad()
        {
            var p1 = new Palette("grayscale.plt", PaletteFormat.Default);
            Assert.IsTrue(AreEqual(p1, Grayscale));

            var p2 = new Palette("grayscale.pal", PaletteFormat.PAL);
            Assert.IsTrue(AreEqual(p2, Grayscale));

            var p3 = new Palette("grayscale.act", PaletteFormat.ACT);
            Assert.IsTrue(AreEqual(p3, Grayscale));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestLoad_Invalid()
        {
            new Palette("grayscale.plt", PaletteFormat.PAL);
        }

        [TestMethod]
        public void TestSave()
        {
            Grayscale.Save("grayscale.plt", PaletteFormat.Default);
            Grayscale.Save("grayscale.pal", PaletteFormat.PAL);
            Grayscale.Save("grayscale.act", PaletteFormat.ACT);

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestSave_Unsupported()
        {
            CreateGrayscale(16).Save("error.act", PaletteFormat.ACT);
        }

        // TODO: test initializing a palette from an indexed image
    }
}
