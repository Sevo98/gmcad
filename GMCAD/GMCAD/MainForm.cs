﻿using System;
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
        string fileName;

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
                image = new Bitmap(openDialog.FileName);
                fileName = openDialog.FileName;
                width = image.Width;
                height = image.Height;
                trackBarScale.Value = 50;
                pictureBox.Image = image;
                height = (image.Height * trackBarScale.Value) / 100;
                width = (image.Width * trackBarScale.Value) / 100;
                pictureBox.Image = ResizeNow(width, height);

            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Unsupported format!");
                return;
            }
            
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

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("Select image!");
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
                MessageBox.Show("Select image!");
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
                MessageBox.Show("Select image!");
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
                MessageBox.Show("Select image!");
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
                MessageBox.Show("Select image!");
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
                    MessageBox.Show("Select image!");
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null || fileName == null)
            {
                MessageBox.Show("Select image!");
                return;
            }

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Save image as...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter = "Images|*.bmp;*.png;*.jpg";
            savedialog.ShowHelp = true;

            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch
                {
                    MessageBox.Show("Unable to save image!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
