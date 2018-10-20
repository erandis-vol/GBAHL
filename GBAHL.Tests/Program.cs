using System;
using System.Windows.Forms;

namespace GBAHL.Tests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Gets or sets the current game file.
        /// </summary>
        public static string GameFilePath { get; set; }
    }
}
