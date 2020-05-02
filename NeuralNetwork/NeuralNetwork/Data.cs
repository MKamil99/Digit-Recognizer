using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NeuralNetwork
{
    class Data
    {
        public static void CheckPrecision(double[][][] datasets, Network network)
        {
            List<double> outputs; int correct = 0;
            for (int i = 0; i < datasets[0].Length; i++)
            {
                network.PushInputValues(datasets[0][i]);
                outputs = network.GetOutput();
                //ClassifyDigit(datasets, outputs, i);
                if (outputs.IndexOf(outputs.Max()) == datasets[1][i].ToList().IndexOf(1)) correct += 1;
            }
            for (int i = 0; i < datasets[2].Length; i++)
            {
                network.PushInputValues(datasets[2][i]);
                outputs = network.GetOutput();
                //ClassifyDigit(datasets, outputs, i);
                if (outputs.IndexOf(outputs.Max()) == datasets[3][i].ToList().IndexOf(1)) correct += 1;
            }
            Console.WriteLine($"\n Correct ones: {correct}/{datasets[0].Length + datasets[2].Length}");
            Console.WriteLine($" Precision: {(Math.Round((double)correct / (datasets[0].Length + datasets[2].Length), 4) * 100).ToString()}%\n");
        }

        private static void ClassifyDigit(double[][][] datasets, List<double> outputs, int row)
        {
            if (row % 10 == 0)
            {
                List<double> expectedoutputs = datasets[1][row].ToList();
                Console.Write(" Should be: ");
                for (int j = 0; j < expectedoutputs.Count; j++) Console.Write(string.Format("{0, 4}", expectedoutputs[j].ToString("0.0")) + " ");
                Console.Write("\n Got:       ");
                for (int j = 0; j < outputs.Count; j++) Console.Write(string.Format("{0, 4}", outputs[j].ToString("0.0")) + " ");
                Console.WriteLine();
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

        private static double[][] LoadFile(string fileName) //from .txt file
        {
            string[] lines = File.ReadAllLines($"data\\{fileName}");
            double[][] data = new double[lines.Length][];

            for(int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(';');
                data[i] = new double[temp.Length];

                for (int j = 0; j < temp.Length; j++)
                    data[i][j] = Convert.ToDouble(temp[j]);
            }

            return data;
        }

        public static void BitmapToTxtFile(Bitmap bitmap)
        {
            double[][] values = new double[bitmap.Height][];
            for (int i = 0; i < values.Length; i++)
                values[i] = new double[bitmap.Width];

            for(int i = 0; i < bitmap.Height; i++)
                for(int j = 0; j < bitmap.Width; j++)
                {
                    values[i][j] = bitmap.GetPixel(j, i).GetBrightness();
                    
                }

            using (var outf = new StreamWriter("test.txt"))
                for (int i = 0; i < values.Length; i++)
                {
                    for (int j = 0; j < values[i].Length - 1; j++)
                        outf.Write(values[i][j] + ";");
                    
                    outf.Write(values[i][values[i].Length - 1]);
                    outf.WriteLine();
                }
        }

        public static void LoadMINSTDataset(string imagesName, string labelsName, double[][] Images, double[][] Labels)
        {
            BinaryReader brImages = new BinaryReader(new FileStream(imagesName, FileMode.Open));
            BinaryReader brLabels = new BinaryReader(new FileStream(labelsName, FileMode.Open));

            int magic1 = Extensions.ReadBigInt32(brImages);
            int numImages = Extensions.ReadBigInt32(brImages);
            int numRows = Extensions.ReadBigInt32(brImages);
            int numCols = Extensions.ReadBigInt32(brImages);

            int magic2 = Extensions.ReadBigInt32(brLabels);
            int numLabels = Extensions.ReadBigInt32(brLabels);

            for (int i = 0; i < 1000; i++) // warunek stopu do zmiany na wcześniejszy; ten tylko do testów
            {
                for (int j = 0; j < numRows * numCols; j++)
                    Images[i][j] = Convert.ToDouble(brImages.ReadByte());

                Labels[i][Convert.ToInt32(brLabels.ReadByte())] = 1;
            }
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