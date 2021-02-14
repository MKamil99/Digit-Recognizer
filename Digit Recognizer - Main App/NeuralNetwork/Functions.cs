using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    class Functions
    {
        public static double Alpha { get; set; } = 0.8;

        public static double InputSumFunction(List<Synapse> Inputs) 
            // funkcja wejścia: suma iloczynów wag synaps wchodzących i wartości wyjściowych neuronów warstwy poprzedniej
        {
            double input = 0;
            foreach (Synapse syn in Inputs) 
                input += syn.GetOutput();
            return input;
        }

        public static double BipolarLinearFunction(double input) // funkcja aktywacji: bipolarna liniowa
            => (1 - Math.Pow(Math.E, -Alpha * input)) / (1 + Math.Pow(Math.E, -Alpha * input));
    }
}