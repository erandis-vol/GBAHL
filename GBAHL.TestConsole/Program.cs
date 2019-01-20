using GBAHL.Asm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GBAHL.TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestPointer();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        static void TestAssemblyWriter()
        {
            using (var aw = new AssemblyWriter() {
                IndentDirectives = true,
                IndentInstructions = true,
                IndentLabels = false,
                IndentParameters = true
            })
            {
                aw.WriteLine("This is a comment.");
                aw.WriteLine("As is this.");
                aw.WriteLine(AssemblyLine.Label("TEST", -1));
                aw.WriteLine(AssemblyLine.Instruction("add", new[] { "r1", "r2", "r2" }, -1));

                Console.WriteLine(aw);
            }
        }

        static void TestPointer()
        {
            GbPtr gbAddress1 = 0x5132;
            GbPtr gbAddress2 = 0x109DCA;
            GbPtr gbAddress3 = GbPtr.Zero;

            Console.WriteLine(Marshal.SizeOf<GbPtr>()); // 3
            Console.WriteLine("{0:X2} {1}", gbAddress1.Bank, gbAddress1.ToString("X6")); // 01 005132
            Console.WriteLine("{0:X2} {1}", gbAddress2.Bank, gbAddress2.ToString("X6")); // 42 109DCA
            Console.WriteLine("{0:X2} {1}", gbAddress3.Bank, gbAddress3.ToString("X6")); // 00 000000

            Ptr gbaAddress1 = 0x0123456;
            Ptr gbaAddress2 = 0x2ABCDEF;
            Ptr gbaAddress3 = Ptr.Invalid;
            Ptr gbaAddress4 = Ptr.Zero;

            Console.WriteLine(Marshal.SizeOf<Ptr>()); // 4

            Console.WriteLine("{0:X2} {1}", gbaAddress1.Bank, gbaAddress1.ToString("X8")); // 08 00123456
            Console.WriteLine("{0:X2} {1}", gbaAddress2.Bank, gbaAddress2.ToString("X8")); // 0A 02ABCDEF
            Console.WriteLine("{0:X2} {1}", gbaAddress3.Bank, gbaAddress3.ToString("X8")); // 00 FFFFFFFF
            Console.WriteLine("{0:X2} {1}", gbaAddress4.Bank, gbaAddress4.ToString("X8")); // 08 00000000
        }
    }
}
