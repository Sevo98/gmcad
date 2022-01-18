using GMvSAPR;
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


        public static Bitmap image;
       // public static Bitmap ImageBitmap;
        private int width;
        private int height;
        string fileName;
        public static uint[,] pixel;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Images|*.bmp;*.png;*.jpg|All files|*.*";
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                if (image != null)
                {
                    image = null;
                }
                image = (Bitmap)Image.FromFile(openDialog.FileName);
                image = null;
                image = new Bitmap(openDialog.FileName);
                fileName = openDialog.FileName;
                width = image.Width;
                height = image.Height;
                trackBarScale.Value = 50;
                pictureBox.Image = image;
                height = (image.Height * trackBarScale.Value) / 100;
                width = (image.Width * trackBarScale.Value) / 100;
                pixel = new uint[image.Height, image.Width];
                for (var y = 0; y < image.Height; y++)
                for (var x = 0; x < image.Width; x++)
                    pixel[y, x] = (uint)(image.GetPixel(x, y).ToArgb());
                pictureBox.Image = ResizeNow(width, height);

            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Unsupported format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }

        //https://youtu.be/eiakPE9R7aw
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

        private void trackBarScale_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trackBarScale.Value = 50;
                return;
            }

            height = (image.Height * trackBarScale.Value) / 100;
            width = (image.Width * trackBarScale.Value) / 100;
            pictureBox.Image = ResizeNow(width, height);
        }

        private void CorrectSizeImage(Image tempImage)
        {
            height = (tempImage.Height * trackBarScale.Value) / 100;
            width = (tempImage.Width * trackBarScale.Value) / 100;
            pictureBox.Image = ResizeNow(width, height);
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Image tempImage = image;
            tempImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            CorrectSizeImage(tempImage);
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Image tempImage = image;
            tempImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            CorrectSizeImage(tempImage);
        }

        private void buttonVertical_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Image tempImage = image;
            tempImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            CorrectSizeImage(tempImage);
        }

        private void buttonHorizontal_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Image tempImage = image;
            tempImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            CorrectSizeImage(tempImage);
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to save the file?", "Save", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (image == null || fileName == null)
                {
                    MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Image saveImage = (Image)pictureBox.Image.Clone();
                saveImage.Save(fileName);
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }

            
        }

        //https://www.youtube.com/watch?v=qGAns8bn7GQ
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null || fileName == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Save image as...";
            saveFile.OverwritePrompt = true;
            saveFile.CheckPathExists = true;
            saveFile.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.PNG)|*.PNG|Image Files(*.JPG)|*.JPG|All files(*.*)|*.*";
            saveFile.ShowHelp = true;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox.Image.Save(saveFile.FileName);
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("Unable to save image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }

        private void trackBarContrast_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trackBarContrast.Value = 0;
                return;
            }
            uint p;
            for (var i = 0; i < image.Height; i++)
            for (var j = 0; j < image.Width; j++)
            {
                p = Contrast.ImageContrast(pixel[i, j], trackBarContrast.Value, trackBarContrast.Maximum);
                image.SetPixel(j, i, Color.FromArgb((int)p));
            }
            pictureBox.Image = image;
        }

        private void buttonContour_Click(object sender, EventArgs e)
        {
            Contour(image, 10);
            pictureBox.Image = image;
        }

        static unsafe float ToGray(byte* bgr)
        {
            return bgr[2] * 0.3f + bgr[1] * 0.59f + bgr[0] * 0.11f;
        }

        public static void Contour(Bitmap b, float threshold)
        {
            var bSrc = (Bitmap)b.Clone();

            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            var stride = bmData.Stride;

            unsafe
            {
                var p = (byte*)(void*)bmData.Scan0;
                var pSrc = (byte*)(void*)bmSrc.Scan0;

                var nOffset = stride - b.Width * 3;
                var nWidth = b.Width - 1;
                var nHeight = b.Height - 1;

                for (var y = 0; y < nHeight; ++y)
                {
                    for (var x = 0; x < nWidth; ++x)
                    {
                        //  | p0 |  p1  |
                        //  |    |  p2  |
                        var p0 = ToGray(pSrc);
                        var p1 = ToGray(pSrc + 3);
                        var p2 = ToGray(pSrc + 3 + stride);

                        if (Math.Abs(p1 - p2) + Math.Abs(p1 - p0) > threshold)
                            p[0] = p[1] = p[2] = 255;
                        else
                            p[0] = p[1] = p[2] = 0;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
        }

        public Bitmap SetBrightness(Bitmap bmap, int brightness)
        {
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            return bmap;
        }
        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trackBarBringness.Value = 0;
                return;
            }

            int brightness = trackBarBringness.Value;

            pictureBox.Image = SetBrightness(image, brightness);
        }

        private void Emboss_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Image is not open. Select image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trackBarBringness.Value = 0;
                return;
            }

            Bitmap embossed = new Bitmap(image); //create a clone
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color c = image.GetPixel(i, j);
                    Color newColor;
                    if ((i == image.Width - 1))
                    {
                        newColor = Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        int x = 0, y = 0;
                        Color next = image.GetPixel(x + 1, y);

                        newColor = Color.FromArgb(
                            Math.Abs((byte)((int)c.R - (int)next.R)),
                            Math.Abs((byte)((int)c.G - (int)next.G)),
                            Math.Abs((byte)((int)c.B - (int)next.B)));

                    }
                    embossed.SetPixel(i, j, newColor);
                }
            }

            pictureBox.Image = embossed;
        }
    }
}
