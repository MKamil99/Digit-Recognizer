using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[][]> data = Data.LoadData("data");
            for (int i = 0; i < data.Count; i++)
                for (int j = 0; j < data[i].Length; j++)
                    for (int k = 0; k < data[i][j].Length; k++)
                        Console.WriteLine(data[i][j][k]);

            Console.ReadKey();
        }
    }
}