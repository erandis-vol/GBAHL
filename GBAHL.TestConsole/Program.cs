using GBAHL.Text;
using GBAHL.Text.Pokemon;
using System;
using System.Linq;

namespace GBAHL.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Write("= ");

                    var line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;

                    var encoded = FireRedEncoding.International.Encode(line);
                    Console.WriteLine("[" + string.Join(", ", encoded.Select(x => x.ToString("X2"))) + "]");

                    var decoded = FireRedEncoding.International.Decode(encoded);
                    Console.WriteLine(decoded);

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
