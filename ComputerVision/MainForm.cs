using System;
using System.Drawing;
using System.Windows.Forms;
using ComputerVision.Entities;
using ComputerVision.Logic;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap image = null;

        private FastImage initialWorkImage;
        private Bitmap initialImage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadClick(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;

            initialImage = new Bitmap(sSourceFileName);
            initialWorkImage = new FastImage(initialImage);
            panelSource.BackgroundImage = initialImage;

            image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);
        }

        private void GrayScaleClick(object sender, EventArgs e)
        {
            Methods.GrayScaleFastImage(workImage);

            UpdateWorkImage();
        }

        private void NegateClick(object sender, EventArgs e)
        {
            Methods.NegateFastImage(workImage);

            UpdateWorkImage();
        }

        private void TrackBarDelta_ValueChanged(object sender, EventArgs e)
        {
            var deltaValue = trackBarDelta.Value;

            workImage.Lock();
            initialWorkImage.Lock();

            for (var i = 0; i < workImage.Width; i++)
            {
                for (var j = 0; j < workImage.Height; j++)
                {
                    var color = initialWorkImage.GetPixel(i, j);

                    var redNew = GetColorBasedOnDelta(color.R, deltaValue);
                    var greenNew = GetColorBasedOnDelta(color.G, deltaValue);
                    var blueNew = GetColorBasedOnDelta(color.B, deltaValue);

                    color = Color.FromArgb(redNew, greenNew, blueNew);

                    workImage.SetPixel(i, j, color);
                }
            }

            UpdateWorkImage();

            workImage.Unlock();
            initialWorkImage.Unlock();
        }

        private byte GetColorBasedOnDelta(byte colorCode, int delta)
        {
            var sum = colorCode + delta;

            if (sum > 255)
            {
                return 255;
            }

            if (sum < 0)
            {
                return 0;
            }

            return (byte)sum;
        }

        private void TrackBarIntensity_ValueChanged(object sender, EventArgs e)
        {
            Methods.ChangeIntensityForFastImage(workImage, initialWorkImage, trackBarIntensity.Value);

            UpdateWorkImage();
        }

        private void ButtonEqualization_Click(object sender, EventArgs e)
        {
            Methods.ApplyEqualization(workImage, initialWorkImage);

            UpdateWorkImage();
        }

        private void UpdateWorkImage()
        {
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();
        }

        private void buttonLowPassFilter_Click(object sender, EventArgs e)
        {
            var n = (int)numericUpDown1.Value;

            Methods.LowPassFiler(workImage, initialWorkImage, n);

            UpdateWorkImage();
        }

        private void buttonResetSecondImage_Click(object sender, EventArgs e)
        {
            image = new Bitmap(initialImage);
            workImage = new FastImage(image);

            UpdateWorkImage();
        }

        private void buttonMarkov_Click(object sender, EventArgs e)
        {
            MarkovFilter.CBPF(workImage, initialWorkImage, 3, 4, 500);

            UpdateWorkImage();
        }

        private void buttonHighPass_Click(object sender, EventArgs e)
        {
            Methods.HighPassFilter(workImage, initialWorkImage);

            UpdateWorkImage();
        }

        private void buttonUnsharp_Click(object sender, EventArgs e)
        {
            Methods.Unsharp(workImage, initialWorkImage);

            UpdateWorkImage();
        }

        private void buttonKirsch_Click(object sender, EventArgs e)
        {
            Methods.Kirsch(workImage, initialWorkImage);

            UpdateWorkImage();
        }

        private void buttonLaplace_Click(object sender, EventArgs e)
        {
            Methods.Laplace(workImage, initialWorkImage);

            UpdateWorkImage();
        }

        private void buttonRoberts_Click(object sender, EventArgs e)
        {
            Methods.Roberts(workImage, initialWorkImage);

            UpdateWorkImage();
        }
    }
}