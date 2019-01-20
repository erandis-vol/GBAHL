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
            Ptr ptr1 = 0x0123456;
            Ptr ptr2 = 0x2ABCDEF;
            Ptr ptr3 = Ptr.Invalid;
            Ptr ptr4 = Ptr.Zero;

            Console.WriteLine(Marshal.SizeOf<Ptr>()); // 4

            Console.WriteLine("{0:X2} {1}", ptr1.Bank, ptr1.ToString("X8")); // 08 00123456
            Console.WriteLine("{0:X2} {1}", ptr2.Bank, ptr2.ToString("X8")); // 0A 02ABCDEF
            Console.WriteLine("{0:X2} {1}", ptr3.Bank, ptr3.ToString("X8")); // 00 FFFFFFFF
            Console.WriteLine("{0:X2} {1}", ptr4.Bank, ptr4.ToString("X8")); // 08 00000000
        }
    }
}
