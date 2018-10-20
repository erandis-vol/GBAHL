using GBAHL.Tests.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBAHL.Tests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            treeViewTests.Nodes.Clear();

            var spriteNode = new TreeNode("Sprites");
            spriteNode.Nodes.Add(new TreeNode {
                Text = "Basic",
                Tag = new BasicSpriteTest()
            });

            var paletteNode = new TreeNode("Palettes");

            treeViewTests.Nodes.Add(spriteNode);
            treeViewTests.Nodes.Add(paletteNode);

            base.OnLoad(e);
        }

        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Title = "Open ROM", Filter = "GBA ROMs|*.gba" })
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                Program.GameFilePath = dialog.FileName;

                Text = $"GBAHL GDI+ Tests - [{Path.GetFileName(Program.GameFilePath)}]";
            }
        }

        private void treeViewTests_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is UserControl testControl)
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(testControl);

                testControl.Dock = DockStyle.Fill;
            }
        }
    }
}
