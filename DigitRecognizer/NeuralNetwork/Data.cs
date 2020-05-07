using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace NeuralNetwork
{
    class Data
    {
        public static double[][][] PrepareDatasets()
        {
            double[][] trainImages = new double[6000][]; // MINST zawiera 60000 elementów, ale nie potrzebujemy tylu cyfr
            double[][] trainLabels = new double[6000][];
            for (int i = 0; i < trainImages.Length; i++)
                trainImages[i] = new double[28 * 28];
            for (int i = 0; i < trainLabels.Length; i++)
                trainLabels[i] = new double[10];

            double[][] testImages = new double[1000][];   // i 10000 tutaj...
            double[][] testLabels = new double[1000][];
            for (int i = 0; i < testImages.Length; i++)
                testImages[i] = new double[28 * 28];
            for (int i = 0; i < testLabels.Length; i++)
                testLabels[i] = new double[10];

            LoadMINSTDataset("train-images.idx3-ubyte", "train-labels.idx1-ubyte", trainImages, trainLabels);
            LoadMINSTDataset("t10k-images.idx3-ubyte", "t10k-labels.idx1-ubyte", testImages, testLabels);

            return new double[][][] { trainImages, trainLabels, testImages, testLabels };
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

        public static List<double[][]> LoadData(string folder)
        {
            List<double[][]> list = new List<double[][]>();
            
            DirectoryInfo diinfo = new DirectoryInfo(folder);
            FileInfo[] Files = diinfo.GetFiles("*.txt");


            foreach(FileInfo file in Files)
            {
                list.Add(LoadFile(file.Name));
            }
             
            return list;
        }

        private static double[][] LoadFile(string fileName) // z pliku .txt
        {
            string[] lines = File.ReadAllLines($"data//{fileName}");
            double[][] data = new double[lines.Length][];

            for(int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(' ');
                data[i] = new double[temp.Length];

                for (int j = 0; j < temp.Length; j++)
                    data[i][j] = Convert.ToDouble(temp[j]);
            }

            return data;
        }

        public static void BitmapToTxtFile(Bitmap bitmap, string path)
        {
            double[][] values = BitmapToArray(bitmap);

            using (var outf = new StreamWriter(path))
                for (int i = 0; i < values.Length; i++)
                {
                    for (int j = 0; j < values[i].Length - 1; j++)
                        outf.Write(values[i][j] + " ");
                    
                    outf.Write(values[i][values[i].Length - 1]);
                    outf.WriteLine();
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

        public static void LoadMINSTDataset(string imagesName, string labelsName, double[][] Images, double[][] Labels)
        {
            BinaryReader brImages = new BinaryReader(new FileStream(imagesName, FileMode.Open));
            BinaryReader brLabels = new BinaryReader(new FileStream(labelsName, FileMode.Open));

            Extensions.ReadBigInt32(brImages);                  // magic1
            int numImages = Extensions.ReadBigInt32(brImages);
            int numRows = Extensions.ReadBigInt32(brImages);
            int numCols = Extensions.ReadBigInt32(brImages);

            Extensions.ReadBigInt32(brLabels);                  // magic2
            Extensions.ReadBigInt32(brLabels);                  // numLabels

            for (int i = 0; i < numImages / 10; i++)
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