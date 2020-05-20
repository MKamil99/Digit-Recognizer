using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace NeuralNetwork
{
    class Network
    {
        static double LearningRate { get; set; } = 0.05;
        static double SynapsesCount;
        internal List<Layer> Layers;

        public Network(double learningrate, double alpha, int inputneuronscount, int[] hiddenlayerssizes, int outputneuronscount)
        {
            if (inputneuronscount < 1 || hiddenlayerssizes.Length < 1 || outputneuronscount < 1)
                throw new Exception("Incorrect Network Parameters");

            Functions.Alpha = alpha;
            LearningRate = learningrate;

            Layers = new List<Layer>();
            AddFirstLayer(inputneuronscount);
            for (int i = 0; i < hiddenlayerssizes.Length; i++)
                AddNextLayer(new Layer(hiddenlayerssizes[i]));
            AddNextLayer(new Layer(outputneuronscount));

            SynapsesCount = CountSynapses();
        }

        private void AddFirstLayer(int inputneuronscount)
        {
            Layer inputlayer = new Layer(inputneuronscount);
            foreach (Neuron neuron in inputlayer.Neurons) 
                neuron.AddInputSynapse(0);
            Layers.Add(inputlayer);
        }

        private void AddNextLayer(Layer newlayer)
        {
            Layer lastlayer = Layers[Layers.Count - 1];
            lastlayer.ConnectLayers(newlayer);
            Layers.Add(newlayer);
        }

        public void PushInputValues(double[] inputs)
        {
            if (inputs.Length != Layers[0].Neurons.Count) 
                throw new Exception("Incorrect Input Size");

            for (int i = 0; i < inputs.Length; i++) 
                Layers[0].Neurons[i].PushValueOnInput(inputs[i]);
        }

        public List<double> GetOutput()
        {
            List<double> output = new List<double>();
            for (int i = 0; i < Layers.Count; i++)
                Layers[i].CalculateOutputOnLayer();
            foreach (Neuron neuron in Layers[Layers.Count - 1].Neurons)
                output.Add(neuron.OutputValue);
            return output;
        }

        public static Network LoadNetworkFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            string[] firstLine = lines[0].Split();
            List<int> hiddenLayerSizes = new List<int>();
            for (int i = 3; i < firstLine.Length - 1; i++)
                hiddenLayerSizes.Add(Convert.ToInt32(firstLine[i]));

            Network net = new Network(double.Parse(firstLine[0]), double.Parse(firstLine[1]), Convert.ToInt32(firstLine[2]),
                hiddenLayerSizes.ToArray(), Convert.ToInt32(firstLine[firstLine.Length - 1]));

            if (lines.Length - 1 != SynapsesCount)
                throw new Exception("Incorrect Input File");
            else
            {
                try
                {
                    int i = 1;
                    for (int j = 1; j < net.Layers.Count; j++)
                        foreach (Neuron neuron in net.Layers[j].Neurons)
                            foreach (Synapse synapse in neuron.Inputs)
                                synapse.Weight = double.Parse(lines[i++]);
                }
                catch (Exception) { throw new Exception("Incorrect Input File"); }
            }
            return net;
        }

        private double CountSynapses()
        {
            double count = 0;
            for (int i = 1; i < Layers.Count; i++)
                foreach (Neuron neuron in Layers[i].Neurons)
                    foreach (Synapse synapse in neuron.Inputs)
                        count++;
            return count;
        }
    }
}