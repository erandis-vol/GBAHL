using GBAHL.Asm;
using GBAHL.Configuration;
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
            TestAssemblyWriter();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public class TestConfigurationData
        {
            public Dictionary<string, int> Offsets { get; set; }

            public int Version { get; set; }
        }

        static void TestJsonConfigurationSave()
        {
            var data = new TestConfigurationData {
                Offsets = new Dictionary<string, int> {
                    ["TEST_1"] = 0x123456,
                    ["TEST_2"] = 0xABCDEF,
                },
                Version = 1,
            };

            using (var sw = new StringWriter())
            {
                JsonConfiguration.Default.Save(sw, data);
                Console.WriteLine(sw.ToString());
            }
        }

        static void TestJsonConfigurationLoad()
        {
            var json = @"
            {
                ""Offsets"": {
                    ""TEST_1"": 0x123456,
                    ""TEST_2"": 0xABCDEF
                },
                ""Version"": 1
            }
            ";

            Console.WriteLine(json);

            var data = JsonConfiguration.Default.Load<TestConfigurationData>(json);

            using (var sw = new StringWriter())
            {
                JsonConfiguration.Default.Save(sw, data);
                Console.WriteLine(sw.ToString());
            }
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
