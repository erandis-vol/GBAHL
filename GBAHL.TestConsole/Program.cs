using GBAHL.Asm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GBAHL.TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            
        }

        public class TestConfigurationData
        {
            public Dictionary<string, int> Offsets { get; set; }

            public int Version { get; set; }
        }

        static void TestAssemblyWriter()
        {
            using (var aw = new AssemblyWriter())
            {
                aw.WriteLine("This is a comment.");
                aw.WriteLine("As is this.");
                aw.WriteLine(AssemblyLine.Label("TEST", -1));
                aw.WriteLine(AssemblyLine.Instruction("add", new[] { "r1", "r2", "r2" }, -1));

                Console.WriteLine(aw);
            }
        }
    }
}
