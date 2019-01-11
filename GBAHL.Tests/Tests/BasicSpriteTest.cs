using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GBAHL.IO;
using System.IO;
using GBAHL.Drawing;
using System.Diagnostics;

namespace GBAHL.Tests.Tests
{
    public partial class BasicSpriteTest : UserControl
    {
        private DirectBitmap image;

        public BasicSpriteTest()
        {
            InitializeComponent();
        }

        ~BasicSpriteTest()
        {
            image?.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tilesetOffset = Convert.ToInt32(textBox1.Text, 16);
            var paletteOffset = Convert.ToInt32(textBox2.Text, 16);

            if (Program.GameFilePath != null)
            {
                var watch = Stopwatch.StartNew();

                Tileset tileset;
                Palette palette;

                // Load the sprite and palette
                using (var gb = new GbaBinaryStream(File.Open(Program.GameFilePath, FileMode.Open, FileAccess.ReadWrite)))
                {
                    gb.Seek(tilesetOffset);
                    tileset = gb.ReadCompressedTiles4();

                    gb.Seek(paletteOffset);
                    palette = gb.ReadPalette(16);
                }

                // Create a new sprite
                var sprite = new Sprite(tileset, palette);
                sprite.Width = 8;

                // Destroy the existing image
                if (image != null)
                {
                    image.Dispose();
                }

                // Display and display the sprite
                pictureBox1.Image = image = SpriteRenderer.Draw(sprite);

                // Collect timing information
                watch.Stop();
                label3.Text = watch.Elapsed.TotalSeconds + " s";
            }
        }
    }
}
