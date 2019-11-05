using System;
using System.Drawing;
using ComputerVision.Entities;

namespace ComputerVision.Logic
{
    public static class MarkovFilter
    {
        public static void CBPF(FastImage fastImage, FastImage originalFastImage, int contextSize, int searchRadius, int sumLimit)
        {
            fastImage.Lock();
            originalFastImage.Lock();

            for (int i = 0; i < originalFastImage.Width; i++)
            {
                for (int j = 0; j < originalFastImage.Height; j++)
                {
                    if (SaltPepper(originalFastImage, i, j))
                    {
                        var newColor = CBP(originalFastImage, i, j, contextSize, searchRadius, sumLimit);
                        fastImage.SetPixel(i, j, Color.FromArgb(newColor, newColor, newColor));
                    }
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        private static byte CBP(FastImage fastImage, int x, int y, int contextSize, int searchRadius, int sumLimit)
        {
            var intensities = new int[256];

            for (int i = x - searchRadius; i <= x + searchRadius; i++)
            {
                if (i < 0 || i >= fastImage.Width)
                {
                    continue;
                }

                for (int j = y - searchRadius; j < y + searchRadius; j++)
                {
                    if (j < 0 || j >= fastImage.Height)
                    {
                        continue;
                    }

                    if (i == y && j == x)
                    {
                        continue;
                    }

                    if (SAD(fastImage, x, y, i, j, contextSize) < sumLimit && !SaltPepper(fastImage, i, j))
                    {
                        var color = fastImage.GetPixel(i, j);
                        intensities[color.R]++;
                    }
                }
            }

            int indexOfBiggestValue = 0;
            var max = 0;

            for (int i = 0; i < intensities.Length; i++)
            {
                if (intensities[i] > max)
                {
                    max = intensities[i];
                    indexOfBiggestValue = i;
                }
            }

            return (byte)indexOfBiggestValue;
        }

        private static int SAD(FastImage fastImage, int x1, int y1, int x2, int y2, int contextSize)
        {
            int sum = 0;

            for (int i = -contextSize / 2; i <= contextSize / 2; i++)
            {
                if (i + x1 < 0 || i + x1 >= fastImage.Width)
                {
                    continue;
                }

                if (i + x2 < 0 || i + x2 >= fastImage.Width)
                {
                    continue;
                }

                for (int j = -contextSize / 2; j < contextSize / 2; j++)
                {
                    if (i + y1 < 0 || i + y1 >= fastImage.Height)
                    {
                        continue;
                    }

                    if (i + y2 < 0 || i + y2 >= fastImage.Height)
                    {
                        continue;
                    }

                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    var color1 = fastImage.GetPixel(j + x1, i + y1);
                    var color2 = fastImage.GetPixel(j + x2, i + y2);
                    sum += Math.Abs(color1.R - color2.R);
                }
            }

            return sum;
        }

        private static bool SaltPepper(FastImage fastImage, int i, int j)
        {
            var color = fastImage.GetPixel(i, j);

            var isBlack = color.R == 0 && color.G == 0 && color.B == 0;
            var isWhite = color.R == 255 && color.G == 255 && color.B == 255;

            return isBlack || isWhite;
        }
    }
}
