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
            List<byte[][]> trainImages = new List<byte[][]>();
            List<byte> trainLabels = new List<byte>();

            List<byte[][]> testImages = new List<byte[][]>();
            List<byte> testLabels = new List<byte>();

            Data.LoadMINSTDataset("train-images.idx3-ubyte", "train-labels.idx1-ubyte", trainImages, trainLabels);
            Data.LoadMINSTDataset("t10k-images.idx3-ubyte", "t10k-labels.idx1-ubyte", testImages, testLabels);

            Data.Shuffle(trainImages, trainLabels);
            

            Console.ReadKey();
            
        }
    }
}