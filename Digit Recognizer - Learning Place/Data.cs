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
            Console.WriteLine(" Loading datasets...");
            string[] arithmeticFilePaths = Directory.GetFiles(@"Datasets\", "signs*.png");
            string[] digitFilePaths      = Directory.GetFiles(@"Datasets\", "digits*.png");
            double[][] trainImages = new double[60000 / MNISTDatasetSizeDivider + arithmeticFilePaths.Length * 180 + digitFilePaths.Length * 120][];
            double[][] trainLabels = new double[60000 / MNISTDatasetSizeDivider + arithmeticFilePaths.Length * 180 + digitFilePaths.Length * 120][];
            for (int i = 0; i < trainImages.Length; i++)
                trainImages[i] = new double[28 * 28];
            for (int i = 0; i < trainLabels.Length; i++)
                trainLabels[i] = new double[14];

            double[][] testImages = new double[10000 / MNISTDatasetSizeDivider + arithmeticFilePaths.Length * 20 + digitFilePaths.Length * 30][];
            double[][] testLabels = new double[10000 / MNISTDatasetSizeDivider + arithmeticFilePaths.Length * 20 + digitFilePaths.Length * 30][];
            for (int i = 0; i < testImages.Length; i++)
                testImages[i] = new double[28 * 28];
            for (int i = 0; i < testLabels.Length; i++)
                testLabels[i] = new double[14];

            LoadMNISTDataset(@"Datasets\train-images.idx3-ubyte", @"Datasets\train-labels.idx1-ubyte", trainImages, trainLabels, MNISTDatasetSizeDivider);
            LoadMNISTDataset(@"Datasets\t10k-images.idx3-ubyte", @"Datasets\t10k-labels.idx1-ubyte", testImages, testLabels, MNISTDatasetSizeDivider);
            LoadOwnDatasets(trainImages, trainLabels, testImages, testLabels, arithmeticFilePaths, digitFilePaths, MNISTDatasetSizeDivider);
            Shuffle(trainImages, trainLabels);

            return new double[][][] { trainImages, trainLabels, testImages, testLabels };
        }

        private static void LoadOwnDatasets(double[][] trainImages, double[][] trainLabels, 
            double[][] testImages, double[][] testLabels, string[] arithmeticFilePaths, string[] digitFilePaths, int MNISTDatasetSizeDivider)
        {
            int trainIndex = 60000 / MNISTDatasetSizeDivider, testIndex = 10000 / MNISTDatasetSizeDivider;

            // Arithmetical files:
            List<double[]> arithmeticSigns; int tempIndex = 0;
            for (int i = 0; i < arithmeticFilePaths.Length; i++)
            {
                arithmeticSigns = RemoveSecondDimensions(DigitDetection.DetectDigits(new Bitmap(arithmeticFilePaths[i])));
                for (int j = 0; j < arithmeticSigns.Count - 20; j++)
                {
                    trainImages[trainIndex] = arithmeticSigns[j];
                    for (int k = 0; k < trainImages[trainIndex].Length; k++)
                    {
                        if (trainImages[trainIndex][k] < 10) trainImages[trainIndex][k] = 0;
                        else trainImages[trainIndex][k] = 1;
                    }
                    trainLabels[trainIndex][(tempIndex++ % 4) + 10] = 1;
                    trainIndex++;
                }
                for (int j = arithmeticSigns.Count - 20; j < arithmeticSigns.Count; j++) // the last 20 signs (which is 10%; files contain 200 signs each) goes to Test Set
                {
                    testImages[testIndex] = arithmeticSigns[j];
                    for (int k = 0; k < testImages[testIndex].Length; k++)
                    {
                        if (testImages[testIndex][k] < 10) testImages[testIndex][k] = 0;
                        else testImages[testIndex][k] = 1;
                    }
                    testLabels[testIndex][(tempIndex++ % 4) + 10] = 1;
                    testIndex++;
                }
            }

            // Digits:
            List<double[]> digits; tempIndex = 0;
            for (int i = 0; i < digitFilePaths.Length; i++)
            {
                digits = RemoveSecondDimensions(DigitDetection.DetectDigits(new Bitmap(digitFilePaths[i])));
                for (int j = 0; j < digits.Count - 30; j++)
                {
                    trainImages[trainIndex] = digits[j];
                    for(int k = 0; k < trainImages[trainIndex].Length; k++)
                    {
                        if (trainImages[trainIndex][k] < 10) trainImages[trainIndex][k] = 0;
                        else trainImages[trainIndex][k] = 1;
                    }
                    trainLabels[trainIndex][(tempIndex++ + 1) % 10] = 1;
                    trainIndex++;
                }
                for (int j = digits.Count - 30; j < digits.Count; j++) // the last 30 signs (which is 20%; files contain 150 each) goes to Test Set
                {
                    testImages[testIndex] = digits[j];
                    for (int k = 0; k < testImages[testIndex].Length; k++)
                    {
                        if (testImages[testIndex][k] < 10) testImages[testIndex][k] = 0;
                        else testImages[testIndex][k] = 1;
                    }
                    testLabels[testIndex][(tempIndex++ + 1) % 10] = 1;
                    testIndex++;
                }
            }
        }

        private static void Shuffle(double[][] arr1, double[][] arr2)
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

        private static void LoadMNISTDataset(string imagesName, string labelsName, double[][] Images, double[][] Labels, int MNISTDatasetSizeDivider)
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
                {
                    Images[i][j] = Convert.ToDouble(brImages.ReadByte());
                    if (Images[i][j] < 10) Images[i][j] = 0;
                    else Images[i][j] = 1;
                }

                Labels[i][Convert.ToInt32(brLabels.ReadByte())] = 1;
            }
        }

        private static List<double[]> RemoveSecondDimensions(List<double[][]> digits)
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