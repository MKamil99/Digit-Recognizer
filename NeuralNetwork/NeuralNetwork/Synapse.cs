using System;

namespace NeuralNetwork
{
    class Synapse
    {
        static Random tmp = new Random();
        internal Neuron FromNeuron, ToNeuron;
        public double Weight { get; set; }
        public double PushedData { get; set; }
        static public int SynapsesCount { get; set; } = 0;

        public Synapse(Neuron fromneuron, Neuron toneuron) // standard synapse
        {
            FromNeuron = fromneuron; ToNeuron = toneuron;
            Weight = tmp.NextDouble() - 0.5;
            SynapsesCount += 1;
        }

        public Synapse(Neuron toneuron, double output)     // input synapse for first layer
        {
            ToNeuron = toneuron; PushedData = output; 
            Weight = 1;
            SynapsesCount += 1;
        }

        public double GetOutput()
        {
            if (FromNeuron == null) return PushedData;     // if it is first layer
            return FromNeuron.OutputValue * Weight;
        }
    }
}