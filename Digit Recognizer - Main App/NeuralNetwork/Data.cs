using System.Collections.Generic;
using System.Drawing;

namespace NeuralNetwork
{
    class Data
    {
        public static double[][] BitmapToArray(Bitmap bitmap)
        {
            double[][] values = new double[bitmap.Height][];
            for (int i = 0; i < values.Length; i++)
                values[i] = new double[bitmap.Width];

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    values[i][j] = 255 - (bitmap.GetPixel(j, i).R + bitmap.GetPixel(j, i).G + bitmap.GetPixel(j, i).B) / 3;

            return values;
        }

        public static List<double[]> RemoveSecondDimensions(List<double[][]> digits)
        {
            List<double[]> tmp = new List<double[]>();
            foreach (double[][] digit in digits)
            {
                List<double> newlist = new List<double>();
                for (int i = 0; i < digit.Length; i++)
                    for (int j = 0; j < digit[i].Length; j++)
                        newlist.Add(digit[i][j]);

                tmp.Add(newlist.ToArray());
            }
            return tmp;
        }
    }
}