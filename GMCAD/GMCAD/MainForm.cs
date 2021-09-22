using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMCAD
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private Image image;
        private int width;
        private int height;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Images|*.bmp;*.png;*.jpg|All files|*.*";
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
            }

            try
            {
                image = new Bitmap(openDialog.FileName);
                width = image.Width;
                height = image.Height;
                pictureBox.Image = image;

            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Unsupported format!");
                return;
            }
            
        }

        private void trackBarHeight_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Select image!");
                trackBarHeight.Value = 100;
                return;
            }

            height = (image.Height * trackBarHeight.Value) / 100;
            pictureBox.Image = ResizeNow(width, height);

            

        }

        private void trackBarWidth_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Select image!");
                trackBarWidth.Value = 100;
                return;
            }

            width = (image.Width * trackBarWidth.Value) / 100;
            pictureBox.Image = ResizeNow(width, height);

        }

        private Bitmap ResizeNow(int target_width, int target_height)
        {
            Rectangle dest_rect = new Rectangle(0, 0, target_width, target_height);
            Bitmap destImage = new Bitmap(target_width, target_height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapmode = new ImageAttributes())
                {
                    wrapmode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, dest_rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapmode);
                }
            }

            return destImage;
        }
    }
}
