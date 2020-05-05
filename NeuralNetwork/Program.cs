using System;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][][] datasets = Data.PrepareDatasets();
            // datasets[0] - Training Set's Input
            // datasets[1] - Training Set's Expected Output
            // datasets[2] - Testing Set's Input
            // datasets[3] - Testing Set's Expected Output

            Network network = new Network(datasets[0][0].Length, 2, 100, datasets[1][0].Length);
            network.LoadWeights("weights.txt");
            network.CalculatePrecision(datasets);
            network.Train(datasets, 1, true);
            network.CalculatePrecision(datasets);

            Console.ReadKey();
        }
    }
}