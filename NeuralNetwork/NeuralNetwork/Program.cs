using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[][]> trainImages = new List<double[][]>();
            List<double> trainLabels = new List<double>();

            List<double[][]> testImages = new List<double[][]>();
            List<double> testLabels = new List<double>();

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