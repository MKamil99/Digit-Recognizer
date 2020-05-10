using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace NeuralNetwork
{
    class Data
    {
        public static double[][][] PrepareDatasets(int MNISTDatasetSizeDivider)
        {
            string[] filePaths = Directory.GetFiles(@"datasets\", "*.png");
            double[][] trainImages = new double[60000 / MNISTDatasetSizeDivider + filePaths.Length * 180][];
            double[][] trainLabels = new double[60000 / MNISTDatasetSizeDivider + filePaths.Length * 180][];
            for (int i = 0; i < trainImages.Length; i++)
                trainImages[i] = new double[28 * 28];
            for (int i = 0; i < trainLabels.Length; i++)
                trainLabels[i] = new double[14];

            double[][] testImages = new double[10000 / MNISTDatasetSizeDivider + filePaths.Length * 20][];
            double[][] testLabels = new double[10000 / MNISTDatasetSizeDivider + filePaths.Length * 20][];
            for (int i = 0; i < testImages.Length; i++)
                testImages[i] = new double[28 * 28];
            for (int i = 0; i < testLabels.Length; i++)
                testLabels[i] = new double[14];

            LoadMNISTDataset(@"datasets\train-images.idx3-ubyte", @"datasets\train-labels.idx1-ubyte", trainImages, trainLabels, MNISTDatasetSizeDivider);
            LoadMNISTDataset(@"datasets\t10k-images.idx3-ubyte", @"datasets\t10k-labels.idx1-ubyte", testImages, testLabels, MNISTDatasetSizeDivider);
            LoadOperationsDataset(trainImages, trainLabels, testImages, testLabels, filePaths, MNISTDatasetSizeDivider);
            Shuffle(trainImages, trainLabels);

            return new double[][][] { trainImages, trainLabels, testImages, testLabels };
        }

        public static void LoadOperationsDataset(double[][] trainImages, double[][] trainLabels, double[][] testImages, double[][] testLabels, string[] filePaths, int MNISTDatasetSizeDivider)
        {
            int trainImageIndex = 60000 / MNISTDatasetSizeDivider, trainLabelsIndex = 60000 / MNISTDatasetSizeDivider, 
                testImageIndex = 10000 / MNISTDatasetSizeDivider, testLabelsIndex = 10000 / MNISTDatasetSizeDivider, tempIndex = 0;
            List<double[]> digits;

            for (int i = 0; i < filePaths.Length; i++)
            {
                digits = RemoveSecondDimensions(DigitDetection.DetectDigits(new Bitmap(filePaths[i])));
                for (int j = 0; j < digits.Count - 20; j++)
                {
                    trainImages[trainImageIndex++] = digits[j];
                    trainLabels[trainLabelsIndex++][(tempIndex++ % 4) + 10] = 1;
                }
                for (int j = digits.Count - 20; j < digits.Count; j++) // ostatnie 20 znaków (czyli 10%, bo mamy pliki po 200 znaków) idzie do testowego
                {
                    testImages[testImageIndex++] = digits[j];
                    testLabels[testLabelsIndex++][(tempIndex++ % 4) + 10] = 1;
                }
            }
        }

        public static void Shuffle(double[][] arr1, double[][] arr2)
        {
            Random rand = new Random();
            int j = arr1.Length;

            while(j > 1)
            {
                int k = rand.Next(j--);
                var temp1 = arr1[j];
                var temp2 = arr2[j];

                arr1[j] = arr1[k];
                arr1[k] = temp1;

                arr2[j] = arr2[k];
                arr2[k] = temp2;
            }
        }

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

        public static void LoadMNISTDataset(string imagesName, string labelsName, double[][] Images, double[][] Labels, int MNISTDatasetSizeDivider)
        {
            BinaryReader brImages = new BinaryReader(new FileStream(imagesName, FileMode.Open));
            BinaryReader brLabels = new BinaryReader(new FileStream(labelsName, FileMode.Open));

            Extensions.ReadBigInt32(brImages);                  // magic1
            int numImages = Extensions.ReadBigInt32(brImages);
            int numRows = Extensions.ReadBigInt32(brImages);
            int numCols = Extensions.ReadBigInt32(brImages);

            Extensions.ReadBigInt32(brLabels);                  // magic2
            Extensions.ReadBigInt32(brLabels);                  // numLabels

            for (int i = 0; i < numImages / MNISTDatasetSizeDivider; i++)
            {
                for (int j = 0; j < numRows * numCols; j++)
                    Images[i][j] = Convert.ToDouble(brImages.ReadByte());

                Labels[i][Convert.ToInt32(brLabels.ReadByte())] = 1;
            }
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

    public static class Extensions
    {
        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}