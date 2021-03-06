﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NeuralNetwork
{
    class DigitDetection
    {
        // Searching for non-white points among the columns:
        private static List<int> ColumnSearch(Bitmap btm)
        {
            List<int> Cols = new List<int>();
            Color color;
            for (int j = 0; j < btm.Width; j++)
                for (int i = 0; i < btm.Height; i++)
                {
                    color = btm.GetPixel(j, i);
                    if (color != Color.FromArgb(255, 255, 255))
                    {
                        Cols.Add(j);
                        break;
                    }
                }
            return Cols;
        }

        // Searching for non-white points among the intervals found by ColumnSearch:
        private static List<int> RowSearch(List<int> StartX, List<int> StopX, Bitmap btm, int digit)
        {
            List<int> Rows = new List<int>();
            Color color;
            for (int k = 0; k < btm.Height; k++)
                for (int j = StartX[digit]; j < StopX[digit]; j++)
                {
                    color = btm.GetPixel(j, k);
                    if (color != Color.FromArgb(255, 255, 255))
                    {
                        Rows.Add(k);
                        break;
                    }
                }
            
            if (Rows.Count == 0)
            {
                Rows.Add(0);
                Rows.Add(btm.Height);
            }

            return Rows;
        }


        private static List<double[][]> IntervalsCounting(List<int> columnsWithBlackPoints, Bitmap btm)
        {
            if (columnsWithBlackPoints.Count == 0)
                return new List<double[][]>();

            // Intervals between columns:
            List<int> StartX = new List<int>();
            List<int> StopX = new List<int>();

            StartX.Add(columnsWithBlackPoints[0]);
            for (int i = 1; i < columnsWithBlackPoints.Count - 1; i++)
                if (columnsWithBlackPoints[i + 1] - columnsWithBlackPoints[i] > 3)
                {
                    StartX.Add(columnsWithBlackPoints[i + 1]);
                    StopX.Add(columnsWithBlackPoints[i]);
                }
            StopX.Add(columnsWithBlackPoints[columnsWithBlackPoints.Count - 1]);


            // Interval between rows:
            List<int> StartY = new List<int>();
            List<int> StopY = new List<int>();
            int digits = StartX.Count; // amount of signs that have benn found
            for (int i = 0; i < digits; i++)
            {
                List<int> Rows = RowSearch(StartX, StopX, btm, i); // for every interval separately
                if (Rows.Count != 0)
                {
                    StartY.Add(Rows[0]);
                    StopY.Add(Rows[Rows.Count - 1]);
                }
            }
            return VerticalCropping(StartX, StopX, StartY, StopY, btm);
        }



        // Cutting out the images and scaling them (called for every interval):
        private static List<double[][]> VerticalCropping(List<int> StartX, List<int> StopX, List<int> StartY, List<int> StopY, Bitmap btm)
        {
            int width, height;
            Bitmap bmpImage = new Bitmap(btm);
            List<double[][]> digits = new List<double[][]>();

            for (int i = 0; i < StartX.Count; i++)
            {
                width = StopX[i] - StartX[i];
                height = StopY[i] - StartY[i];
                if (width > 0 && height > 0)
                {
                    Bitmap bmpCrop = bmpImage.Clone(new Rectangle(StartX[i], StartY[i], width, height), bmpImage.PixelFormat);
                    digits.Add(ResizeImage(TransformToSquare(bmpCrop, btm)));
                }
            }
            return digits;
        }

        private static Bitmap TransformToSquare(Bitmap bmpCrop, Bitmap btm)
        {
            int height, width;
            if (bmpCrop.Height < btm.Height * 0.15 && bmpCrop.Width < btm.Height * 0.15)
            {
                height = (int)(bmpCrop.Height * 8);
                width = (int)(bmpCrop.Height * 8);
            }
            else if (bmpCrop.Height > bmpCrop.Width)
            {
                height = (int)(bmpCrop.Height * 1.5);
                width = (int)(bmpCrop.Height * 1.5);
            }
            else
            {
                height = (int)(bmpCrop.Width * 1.5);
                width = (int)(bmpCrop.Width * 1.5);
            }
            Bitmap bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, 0, 0, width, height);
                int x = width / 2 - bmpCrop.Width / 2;
                int y = height / 2 - bmpCrop.Height / 2;
                g.DrawImage(bmpCrop, x, y);
            }
            return bitmap;
        }

        // Changing the dimensions of image to 28x28 and returning the bitmap as two-dimensional array:
        private static double[][] ResizeImage(Image image)
        {
            int width = 28, height = 28;
            Rectangle croppSize = new Rectangle(0, 0, width, height);
            Bitmap resizedImage = new Bitmap(width, height);
            resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, croppSize, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return Data.BitmapToArray(resizedImage);
        }

        public static List<double[][]> DetectDigits(Bitmap picture) => IntervalsCounting(ColumnSearch(picture), picture);
    }
}