using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args) //60 000   i    10 000
        {
            double[][] trainImages = new double[60000][];
            double[] trainLabels = new double[60000];
            for (int i = 0; i < trainImages.Length; i++)
                trainImages[i] = new double[28 * 28];

            double[][] testImages = new double[10000][];
            double[] testLabels = new double[10000];
            for (int i = 0; i < testImages.Length; i++)
                testImages[i] = new double[28 * 28];
            
            Console.WriteLine("Loading data...");

            Data.LoadMINSTDataset("train-images.idx3-ubyte", "train-labels.idx1-ubyte", trainImages, trainLabels);
            Data.LoadMINSTDataset("t10k-images.idx3-ubyte", "t10k-labels.idx1-ubyte", testImages, testLabels);

            Data.Shuffle(trainImages, trainLabels);
            Data.Shuffle(testImages, testLabels);

            Console.WriteLine("Creating neural network...");
            Network network = new Network(28 * 28, 5, 5, 1);





            Console.WriteLine("Completed!");
            Console.ReadKey();
        }
    }
}