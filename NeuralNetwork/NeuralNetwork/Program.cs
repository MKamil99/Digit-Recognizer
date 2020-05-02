using System;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][] trainImages = new double[1000][]; // 60 tysięcy domyślnie; zmniejszone na potrzeby testu
            double[][] trainLabels = new double[1000][];
            for (int i = 0; i < trainImages.Length; i++)
                trainImages[i] = new double[28 * 28];
            for (int i = 0; i < trainLabels.Length; i++)
                trainLabels[i] = new double[10];

            double[][] testImages = new double[1000][]; // 10 tysięcy domyślnie; zmniejszone na potrzeby testu
            double[][] testLabels = new double[1000][];
            for (int i = 0; i < testImages.Length; i++)
                testImages[i] = new double[28 * 28];
            for (int i = 0; i < testLabels.Length; i++)
                testLabels[i] = new double[10];
            
            Console.WriteLine(" Loading data...");

            Data.LoadMINSTDataset("train-images.idx3-ubyte", "train-labels.idx1-ubyte", trainImages, trainLabels);
            Data.LoadMINSTDataset("t10k-images.idx3-ubyte", "t10k-labels.idx1-ubyte", testImages, testLabels);

            //Data.Shuffle(trainImages, trainLabels);
            double[][][] dataset = new double[][][] { trainImages, trainLabels, testImages, testLabels };

            Network network = new Network(trainImages[0].Length, 2, 100, trainLabels[0].Length);
            Data.CheckPrecision(dataset, network);
            network.Train(dataset, 100);
            Data.CheckPrecision(dataset, network);

            Console.ReadKey();
        }
    }
}