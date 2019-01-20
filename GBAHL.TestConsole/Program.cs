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
            GbaPtr address1 = 0x0123456;
            GbaPtr address2 = 0x2ABCDEF;
            GbaPtr address3 = GbaPtr.Invalid;
            GbaPtr address4 = GbaPtr.Zero;

            Console.WriteLine(Marshal.SizeOf<GbaPtr>()); // 4

            Console.WriteLine("{0:X2}", address1.Bank); // 08
            Console.WriteLine("{0:X2}", address2.Bank); // 0A
            Console.WriteLine("{0:X2}", address3.Bank); // 00
            Console.WriteLine("{0:X2}", address4.Bank); // 08
        }
    }
}
