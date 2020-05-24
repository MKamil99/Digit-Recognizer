using System;

namespace NeuralNetwork
{
    class Program
    {
        static void Main()
        {
            int MNISTDatasetSizeDivider = 50; // 1 -> 60000+10000; 5 -> 12000+2000; 10 -> 6000+1000; itd.
            double[][][] datasets = Data.PrepareDatasets(MNISTDatasetSizeDivider);
            // datasets[0] - Training Set's Input
            // datasets[1] - Training Set's Expected Output
            // datasets[2] - Testing  Set's Input
            // datasets[3] - Testing  Set's Expected Output

            //Network network = new Network(0.8, datasets[0][0].Length, new int[] { 100, 100, 100, 100}, datasets[1][0].Length);
            Network network = Network.LoadNetworkFromFile("weights.txt");
            network.CalculatePrecision(datasets);
            network.Train(datasets, 15, true);
            network.CalculatePrecision(datasets);
            //for (int i = 0; i < datasets[2].Length; i++)
            //{
            //    network.PushInputValues(datasets[2][i]);
            //    var outputs = network.GetOutput();
            //    network.Classify(datasets[3][i], outputs);
            //}

            Console.ReadKey();
        }
    }
}