using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ComputerVision.Entities;

namespace ComputerVision.Logic
{
    public static class Methods
    {
        public static void GrayScaleFastImage(FastImage fastImage)
        {
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = fastImage.GetPixel(i, j);
                    var average = (byte)((color.R + color.G + color.B) / 3);

                    color = Color.FromArgb(average, average, average);

                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
        }

        public static void NegateFastImage(FastImage fastImage)
        {
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = fastImage.GetPixel(i, j);

                    var newRed = (byte)(255 - color.R);
                    var newGreen = (byte)(255 - color.G);
                    var newBlue = (byte)(255 - color.B);

                    color = Color.FromArgb(newRed, newGreen, newBlue);

                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
        }

        public static void ChangeIntensityForFastImage(FastImage fastImage, FastImage originalFastImage, int intensity)
        {
            fastImage.Lock();
            originalFastImage.Lock();

            var minR = originalFastImage.RedMinimumValue;
            var maxR = originalFastImage.RedMaximumValue;
            var minG = originalFastImage.GreenMinimumValue;
            var maxG = originalFastImage.GreenMaximumValue;
            var minB = originalFastImage.BlueMinimumValue;
            var maxB = originalFastImage.BlueMaximumValue;

            var redA = GetA(minR, intensity);
            var redB = GetB(maxR, intensity);
            var greenA = GetA(minG, intensity);
            var greenB = GetB(maxG, intensity);
            var blueA = GetA(minB, intensity);
            var blueB = GetB(maxB, intensity);

            for (var i = 0; i < originalFastImage.Width; i++)
            {
                for (var j = 0; j < originalFastImage.Height; j++)
                {
                    var oldRed = originalFastImage.GetPixel(i, j).R;
                    var newRed = (redB - redA) * (oldRed - minR) / (maxR - minR) + redA;

                    var oldGreen = originalFastImage.GetPixel(i, j).G;
                    var newGreen = (greenB - greenA) * (oldGreen - minG) / (maxG - minG) + greenA;

                    var oldBlue = originalFastImage.GetPixel(i, j).B;
                    var newBlue = (blueB - blueA) * (oldBlue - minB) / (maxB - minB) + blueA;

                    if (newRed > 255)
                    {
                        newRed = 255;
                    }
                    else if (newRed < 0)
                    {
                        newRed = 0;
                    }

                    if (newGreen > 255)
                    {
                        newGreen = 255;
                    }
                    else if (newGreen < 0)
                    {
                        newGreen = 0;
                    }

                    if (newBlue > 255)
                    {
                        newBlue = 255;
                    }
                    else if (newBlue < 0)
                    {
                        newBlue = 0;
                    }

                    var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    fastImage.SetPixel(i, j, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        private static int GetA(byte min, int delta)
        {
            return min - delta;
        }

        private static int GetB(byte max, int delta)
        {
            return max + delta;
        }

        public static void ApplyEqualization(FastImage fastImage, FastImage originalFastImage)
        {
            var oldGrayScaleHistogram = originalFastImage.GrayScaleHistogram;
            var newGrayScaleHistogram = new int[256];
            newGrayScaleHistogram[0] = oldGrayScaleHistogram[0];

            for (var i = 1; i < oldGrayScaleHistogram.Length; i++)
            {
                newGrayScaleHistogram[i] = newGrayScaleHistogram[i - 1] + oldGrayScaleHistogram[i];
            }

            var transf = new int[256];
            for (var i = 0; i < transf.Length; i++)
            {
                transf[i] = (newGrayScaleHistogram[i] * 255) / (originalFastImage.Width * originalFastImage.Height);
            }

            originalFastImage.Lock();
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = originalFastImage.GetPixel(i, j);
                    var gray = (color.R + color.G + color.B) / 3;
                    var newColor = Color.FromArgb(transf[gray], transf[gray], transf[gray]);

                    fastImage.SetPixel(i, j, newColor);
                }
            }

            originalFastImage.Unlock();
            fastImage.Unlock();
        }

        public static void LowPassFiler(FastImage fastImage, FastImage originalFastImage, int n)
        {
            if (n < 1)
            {
                throw new ArgumentException();
            }

            var matrix = GetLowPassFilterMatrix(n);

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    var sumRed = 0;
                    var sumGreen = 0;
                    var sumBlue = 0;

                    for (int i = row - 1; i <= row + 1; i++)
                    {
                        for (int j = column - 1; j <= column + 1; j++)
                        {
                            var pixel = originalFastImage.GetPixel(i, j);
                            sumRed += pixel.R * matrix[i - row + 1, j - column + 1];
                            sumGreen += pixel.G * matrix[i - row + 1, j - column + 1];
                            sumBlue += pixel.B * matrix[i - row + 1, j - column + 1];
                        }
                    }

                    var divide = (n + 2) * (n + 2);
                    var newRed = sumRed / divide;
                    var newGreen = sumGreen / divide;
                    var newBlue = sumBlue / divide;
                    var newColor = Color.FromArgb(newRed, newGreen, newBlue);

                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void HighPassFilter(FastImage fastImage, FastImage originalFastImage)
        {
            int[,] H =
            {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };

            fastImage.Lock();
            originalFastImage.Lock();
            for (int i = 1; i <= fastImage.Width - 2; i++)
            {
                for (int j = 1; j <= fastImage.Height - 2; j++)
                {
                    var sumaR = 0;
                    var sumaG = 0;
                    var sumaB = 0;
                    Color color;

                    for (int row = i - 1; row <= i + 1; row++)
                    {
                        for (int col = j - 1; col <= j + 1; col++)
                        {
                            color = originalFastImage.GetPixel(row, col);
                            int R = color.R;
                            int G = color.G;
                            int B = color.B;

                            sumaR += R * H[row - i + 1, col - j + 1];
                            sumaG += G * H[row - i + 1, col - j + 1];
                            sumaB += B * H[row - i + 1, col - j + 1];
                        }
                    }
                    if (sumaR > 255)
                    {
                        sumaR = 255;
                    }
                    if (sumaR < 0)
                    {
                        sumaR = 0;
                    }
                    if (sumaG > 255)
                    {
                        sumaG = 255;
                    }
                    if (sumaG < 0)
                    {
                        sumaG = 0;
                    }
                    if (sumaB > 255)
                    {
                        sumaB = 255;
                    }
                    if (sumaB < 0)
                    {
                        sumaB = 0;
                    }

                    color = Color.FromArgb(sumaR, sumaG, sumaB);
                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Unsharp(FastImage fastImage, FastImage originalFastImage)
        {
            int[,] H = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            double c = 0.6;

            fastImage.Lock();
            originalFastImage.Lock();
            for (int i = 1; i < fastImage.Width - 2; i++)
            {
                for (int j = 1; j < fastImage.Height - 2; j++)
                {
                    double sumR = 0;
                    double sumG = 0;
                    double sumB = 0;
                    double SumaR0 = 0;
                    double SumaG0 = 0;
                    double SumaB0 = 0;
                    double R = 0;
                    double G = 0;
                    double B = 0;
                    Color color;
                    for (int row = i - 1; row <= i + 1; row++)
                    {
                        for (int col = j - 1; col <= j + 1; col++)
                        {
                            color = originalFastImage.GetPixel(row, col);
                            R = color.R;
                            G = color.G;
                            B = color.B;

                            sumR = sumR + R * H[row - i + 1, col - j + 1];
                            sumG = sumG + G * H[row - i + 1, col - j + 1];
                            sumB = sumB + B * H[row - i + 1, col - j + 1];
                        }
                    }
                    sumR /= (1 + 2) * (1 + 2);
                    sumG /= (1 + 2) * (1 + 2);
                    sumB /= (1 + 2) * (1 + 2);

                    SumaR0 = (c / (1.2 - 1.0)) * R - ((1.0 - c) / (1.2 - 1.0)) * sumR;
                    SumaG0 = (c / (1.2 - 1.0)) * G - ((1.0 - c) / (1.2 - 1.0)) * sumG;
                    SumaB0 = (c / (1.2 - 1.0)) * B - ((1.0 - c) / (1.2 - 1.0)) * sumB;

                    if (SumaR0 > 255)
                    {
                        SumaR0 = 255;
                    }
                    if (SumaR0 < 0)
                    {
                        SumaR0 = 0;
                    }
                    if (SumaG0 > 255)
                    {
                        SumaG0 = 255;
                    }
                    if (SumaG0 < 0)
                    {
                        SumaG0 = 0;
                    }
                    if (SumaB0 > 255)
                    {
                        SumaB0 = 255;
                    }
                    if (SumaB0 < 0)
                    {
                        SumaB0 = 0;
                    }

                    color = Color.FromArgb((int)SumaR0, (int)SumaG0, (int)SumaB0);
                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Kirsch(FastImage fastImage, FastImage originalFastImage)
        {
            var H1 = new[,]
            {
                {-1, 0, 1},
                {-1, 0, 1},
                {-1, 0, 1}
            };

            var H2 = new[,]
            {
                {1, 1, 1},
                {0, 0, 0},
                {-1, -1, -1}
            };

            var H3 = new[,]
            {
                {0, 1, 1},
                {-1, 0, 1},
                {-1, -1, 0}
            };

            var H4 = new[,]
            {
                {1, 1, 0},
                {1, 0, -1},
                {0, -1, -1}
            };

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSums(originalFastImage, row, column, H1, out var sumRedH1, out var sumGreenH1, out var sumBlueH1);
                    GetConvolutionSums(originalFastImage, row, column, H2, out var sumRedH2, out var sumGreenH2, out var sumBlueH2);
                    GetConvolutionSums(originalFastImage, row, column, H3, out var sumRedH3, out var sumGreenH3, out var sumBlueH3);
                    GetConvolutionSums(originalFastImage, row, column, H4, out var sumRedH4, out var sumGreenH4, out var sumBlueH4);

                    var sumsRed = new List<int> { sumRedH1, sumRedH2, sumRedH3, sumRedH4 };
                    var sumsGreen = new List<int> { sumGreenH1, sumGreenH2, sumGreenH3, sumGreenH4 };
                    var sumsBlue = new List<int> { sumBlueH1, sumBlueH2, sumBlueH3, sumBlueH4 };

                    var maxRed = sumsRed.Max();
                    var maxGreen = sumsGreen.Max();
                    var maxBlue = sumsBlue.Max();

                    maxRed = Normalizare(maxRed, 0, 255);
                    maxGreen = Normalizare(maxGreen, 0, 255);
                    maxBlue = Normalizare(maxBlue, 0, 255);

                    var newColor = Color.FromArgb(maxRed, maxGreen, maxBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Laplace(FastImage fastImage, FastImage originalFastImage)
        {
            var H1 = new[,]
            {
                {0, 1, 0},
                {1, -4, 1},
                {0, 1, 0}
            };

            var H2 = new[,]
            {
                {-1, -1, -1},
                {-1, 8, -1},
                {-1, -1, -1}
            };

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSums(originalFastImage, row, column, H1, out var sumRedH1, out var sumGreenH1, out var sumBlueH1);
                    GetConvolutionSums(originalFastImage, row, column, H2, out var sumRedH2, out var sumGreenH2, out var sumBlueH2);

                    var sumsRed = new List<int> { sumRedH1, sumRedH2 };
                    var sumsGreen = new List<int> { sumGreenH1, sumGreenH2 };
                    var sumsBlue = new List<int> { sumBlueH1, sumBlueH2 };

                    var maxRed = sumsRed.Max();
                    var maxGreen = sumsGreen.Max();
                    var maxBlue = sumsBlue.Max();

                    maxRed = Normalizare(maxRed, 0, 255);
                    maxGreen = Normalizare(maxGreen, 0, 255);
                    maxBlue = Normalizare(maxBlue, 0, 255);

                    var newColor = Color.FromArgb(maxRed, maxGreen, maxBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Roberts(FastImage fastImage, FastImage originalFastImage)
        {
            var P = new[,]
            {
                {-1, 0},
                {0, 1},
            };

            var Q = new[,]
            {
                {0, 1},
                {-1, 0},
            };

            fastImage.Lock();
            originalFastImage.Lock();

            const int k = 7;

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSumsForRobert(originalFastImage, row, column, P, out var sumRedP, out var sumGreenP, out var sumBlueP);
                    GetConvolutionSumsForRobert(originalFastImage, row, column, Q, out var sumRedQ, out var sumGreenQ, out var sumBlueQ);

                    int newRed = k * (int)Math.Sqrt(Math.Pow(sumRedP, 2) + Math.Pow(sumRedQ, 2));
                    int newGreen = k * (int)Math.Sqrt(Math.Pow(sumGreenP, 2) + Math.Pow(sumGreenQ, 2));
                    int newBlue = k * (int)Math.Sqrt(Math.Pow(sumBlueP, 2) + Math.Pow(sumBlueQ, 2));

                    newRed = Normalizare(newRed, 0, 255);
                    newGreen = Normalizare(newGreen, 0, 255);
                    newBlue = Normalizare(newBlue, 0, 255);

                    var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Prewitt(FastImage fastImage, FastImage originalFastImage)
        {
            var P = new[,]
            {
                {-1, -1, -1},
                {0, 0, 0},
                {1, 1, 1}
            };

            var Q = new[,]
            {
                {-1, 0, 1},
                {-1, 0, 1},
                {-1, 0, 1}
            };

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSums(originalFastImage, row, column, P, out var sumRedP, out var sumGreenP, out var sumBlueP);
                    GetConvolutionSums(originalFastImage, row, column, Q, out var sumRedQ, out var sumGreenQ, out var sumBlueQ);

                    var maxRed = (int)Math.Sqrt(Math.Pow(sumRedP, 2) + Math.Pow(sumRedQ, 2));
                    var maxGreen = (int)Math.Sqrt(Math.Pow(sumGreenP, 2) + Math.Pow(sumGreenQ, 2));
                    var maxBlue = (int)Math.Sqrt(Math.Pow(sumBlueP, 2) + Math.Pow(sumBlueQ, 2));

                    maxRed = Normalizare(maxRed, 0, 255);
                    maxGreen = Normalizare(maxGreen, 0, 255);
                    maxBlue = Normalizare(maxBlue, 0, 255);

                    var newColor = Color.FromArgb(maxRed, maxGreen, maxBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void Sobel(FastImage fastImage, FastImage originalFastImage)
        {
            var P = new[,]
            {
                {-1, -2, -1},
                {0, 0, 0},
                {1, 2, 1}
            };

            var Q = new[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSums(originalFastImage, row, column, P, out var sumRedP, out var sumGreenP, out var sumBlueP);
                    GetConvolutionSums(originalFastImage, row, column, Q, out var sumRedQ, out var sumGreenQ, out var sumBlueQ);

                    var maxRed = (int)Math.Sqrt(Math.Pow(sumRedP, 2) + Math.Pow(sumRedQ, 2));
                    var maxGreen = (int)Math.Sqrt(Math.Pow(sumGreenP, 2) + Math.Pow(sumGreenQ, 2));
                    var maxBlue = (int)Math.Sqrt(Math.Pow(sumBlueP, 2) + Math.Pow(sumBlueQ, 2));

                    maxRed = Normalizare(maxRed, 0, 255);
                    maxGreen = Normalizare(maxGreen, 0, 255);
                    maxBlue = Normalizare(maxBlue, 0, 255);

                    var newColor = Color.FromArgb(maxRed, maxGreen, maxBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        public static void FreiChen(FastImage fastImage, FastImage originalFastImage)
        {
            var matrixes = new List<double[,]>();
            var radical2 = Math.Sqrt(2);

            //F1
            matrixes.Add(new double[,]
            {
                {1, radical2, 1 },
                {0, 0, 0 },
                {-1, -radical2, -1 }
            });

            // F2
            matrixes.Add(new double[,]
            {
                {1, 0, -1 },
                {radical2, 0, -radical2 },
                {1, 0, -1 }
            });

            // F3
            matrixes.Add(new double[,]
            {
                {0, -1, radical2 },
                {1, 0, -1 },
                {-radical2, 1, 0 }
            });

            // F4
            matrixes.Add(new double[,]
            {
                {radical2, -1, 0 },
                {-1, 0, 1 },
                {0, 1, -radical2 }
            });

            // F5
            matrixes.Add(new double[,]
            {
                {0, 1, 0 },
                {-1, 0, -1 },
                {0, 1, 0 }
            });

            // F6
            matrixes.Add(new double[,]
            {
                {-1, 0, 1 },
                {0, 0, 0 },
                {1, 0, -1 }
            });

            // F7
            matrixes.Add(new double[,]
            {
                {1, -2, 1 },
                {-2, 4, -2 },
                {1, -2, 1 }
            });

            // F8
            matrixes.Add(new double[,]
            {
                {-2, 1, -2 },
                {1, 4, 1 },
                {-2, 1, -2 }
            });

            // F9
            matrixes.Add(new double[,]
            {
                {1d/9, 1d/9, 1d/9 },
                {1d/9, 1d/9, 1d/9 },
                {1d/9, 1d/9, 1d/9}
            });

            fastImage.Lock();
            originalFastImage.Lock();

            for (int row = 1; row < originalFastImage.Width - 2; row++)
            {
                for (int column = 1; column < originalFastImage.Height - 2; column++)
                {
                    GetConvolutionSumsForFreiChen(originalFastImage, row, column, matrixes, out var sumRed, out var sumGreen, out var sumBlue);

                    sumRed = Normalizare(sumRed, 0, 255);
                    sumGreen = Normalizare(sumGreen, 0, 255);
                    sumBlue = Normalizare(sumBlue, 0, 255);

                    var newColor = Color.FromArgb(sumRed, sumGreen, sumBlue);
                    fastImage.SetPixel(row, column, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        private static void GetConvolutionSums(FastImage fastImage, int x, int y, int[,] matrix, out int sumRed, out int sumGreen, out int sumBlue)
        {
            sumRed = 0;
            sumGreen = 0;
            sumBlue = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    var pixel = fastImage.GetPixel(i, j);
                    sumRed += pixel.R * matrix[i - x + 1, j - y + 1];
                    sumGreen += pixel.G * matrix[i - x + 1, j - y + 1];
                    sumBlue += pixel.B * matrix[i - x + 1, j - y + 1];
                }
            }
        }

        private static void GetConvolutionSumsForRobert(FastImage fastImage, int x, int y, int[,] matrix, out int sumRed, out int sumGreen, out int sumBlue)
        {
            sumRed = 0;
            sumGreen = 0;
            sumBlue = 0;

            for (int i = x; i <= x + 1; i++)
            {
                for (int j = y; j <= y + 1; j++)
                {
                    var pixel = fastImage.GetPixel(i, j);
                    sumRed += pixel.R * matrix[i - x, j - y];
                    sumGreen += pixel.G * matrix[i - x, j - y];
                    sumBlue += pixel.B * matrix[i - x, j - y];
                }
            }
        }

        private static void GetConvolutionSumsForFreiChen(FastImage fastImage, int x, int y, List<double[,]> matrixes, out int sumRed, out int sumGreen, out int sumBlue)
        {
            var firstSumR = 0d;
            var firstSumG = 0d;
            var firstSumB = 0d;
            var secondSumR = 0d;
            var secondSumG = 0d;
            var secondSumB = 0d;

            for (var index = 0; index < matrixes.Count; index++)
            {
                var matrix = matrixes[index];

                for (int i = x; i <= x + 1; i++)
                {
                    for (int j = y; j <= y + 1; j++)
                    {
                        var pixel = fastImage.GetPixel(i, j);
                        secondSumR += Math.Pow(pixel.R * matrix[i - x, j - y], 2);
                        secondSumG += Math.Pow(pixel.G * matrix[i - x, j - y], 2);
                        secondSumB += Math.Pow(pixel.B * matrix[i - x, j - y], 2);

                        if (index < 4)
                        {
                            firstSumR += Math.Pow(pixel.R * matrix[i - x, j - y], 2);
                            firstSumG += Math.Pow(pixel.G * matrix[i - x, j - y], 2);
                            firstSumB += Math.Pow(pixel.B * matrix[i - x, j - y], 2);
                        }
                    }
                }
            }

            sumRed = (int)(Math.Sqrt(firstSumR / secondSumR) * 255);
            sumGreen = (int)(Math.Sqrt(firstSumG / secondSumG) * 255);
            sumBlue = (int)(Math.Sqrt(firstSumB / secondSumB) * 255);
        }

        private static int Normalizare(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        private static int[,] GetLowPassFilterMatrix(int n)
        {
            return new int[,]
            {
                { 1, n, 1 },
                { n, n * n, n },
                { 1, n, 1 }
            };
        }
    }
}
