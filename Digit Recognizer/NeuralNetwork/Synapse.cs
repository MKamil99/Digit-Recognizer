using System;

namespace NeuralNetwork
{
    class Synapse
    {
        static readonly Random tmp = new Random();
        internal Neuron FromNeuron, ToNeuron;
        public double Weight { get; set; }
        public double PushedData { get; set; }

        public Synapse(Neuron fromneuron, Neuron toneuron) // zwykła synapsa
        {
            FromNeuron = fromneuron; ToNeuron = toneuron;
            Weight = tmp.NextDouble() - 0.5;
        }

        public Synapse(Neuron toneuron, double output)     // synapsa wejściowa dla pierwszej warstwy
        {
            ToNeuron = toneuron; PushedData = output; 
            Weight = 1;
        }

        public double GetOutput()
        {
            if (FromNeuron == null) return PushedData;     // jeśli to pierwsza warstwa
            return FromNeuron.OutputValue * Weight;
        }
    }
}