using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBAHL.Tests.Tests
{
    public partial class GbaFileInfoTest : UserControl
    {
        public GbaFileInfoTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Program.GameFilePath != null)
            {
                propertyGrid1.SelectedObject = new GbaFileInfo(Program.GameFilePath);
            }
        }
    }
}
