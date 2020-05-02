using System;
using System.Collections.Generic;
using System.IO;

namespace NeuralNetwork
{
    class Network
    {
        static double LearningRate = 0.1;
        internal List<Layer> Layers;
        internal double[][] ExpectedResult;
        double[][] differences;

        public Network(int inputneuronscount, int hiddenlayerscount, int hiddenneuronscount, int outputneuronscount, string path = null)
        {
            Console.WriteLine(" Building neural network...");
            if (inputneuronscount < 1 || hiddenlayerscount < 1 || hiddenneuronscount < 1 || outputneuronscount < 1)
                throw new Exception("Incorrect Network Parameters");

            Layers = new List<Layer>();
            AddFirstLayer(inputneuronscount);
            for (int i = 0; i < hiddenlayerscount; i++) 
                AddNextLayer(new Layer(hiddenneuronscount));
            AddNextLayer(new Layer(outputneuronscount));

            differences = new double[Layers.Count][];
            for (int i = 1; i < Layers.Count; i++) 
                differences[i] = new double[Layers[i].Neurons.Count];

            if (File.Exists(path))
            {
                Console.WriteLine(" Loading weights...");
                string[] lines = File.ReadAllLines(path);
                if (lines.Length != Synapse.SynapsesCount)
                    Console.WriteLine(" Incorrect input file.");
                else LoadWeights(lines);
            }
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

        public void PushExpectedValues(double[][] expectedvalues) 
        {
            if (expectedvalues[0].Length != Layers[Layers.Count - 1].Neurons.Count) 
                throw new Exception("Incorrect Expected Output Size");

            ExpectedResult = expectedvalues;
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

        public void Train(double[][][] datasets, double epochscount)
        {
            double[][] trainingInputs = datasets[0], trainingOutputs = datasets[1];
            double[][] testingInputs = datasets[2], testingOutputs = datasets[3];
            PushExpectedValues(trainingOutputs);

            Console.WriteLine(" Training neural network...");
            double recenterror = double.MaxValue, minerror = double.MaxValue;  // comment this, 99th and 101th lines to turn off informing about an error
            for (int i = 0; i < epochscount; i++)
            {
                List<double> outputs = new List<double>();
                for (int j = 0; j < trainingInputs.Length; j++)
                {
                    PushInputValues(trainingInputs[j]);
                    outputs = GetOutput();
                    ChangeWeights(outputs, j);
                }
                recenterror = Test(testingInputs, testingOutputs);
                //if (minerror < recenterror) break;                           // uncomment this line to turn on breaking the loop
                minerror = recenterror;
            }
            SaveWeights(@"weights.txt");
            Console.WriteLine(" Done!");
        }

        private double Test(double[][] inputs, double[][] expectedoutputs)
        {
            double error = 0; 
            List<double> outputs = new List<double>();
            for (int i = 0; i < inputs.Length; i++)
            {
                PushInputValues(inputs[i]);
                outputs = GetOutput();
                error += Functions.CalculateError(outputs, i, expectedoutputs);
            }
            error /= inputs.Length;
            Console.WriteLine($" Average mean square error: {Math.Round(error, 5)}");
            return error;
        }

        private void CalculateDifferences(List<double> outputs, int row)
        {
            for (int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
                differences[Layers.Count - 1][i] = (ExpectedResult[row][i] - outputs[i]) 
                    * Functions.BipolarDifferential(Layers[Layers.Count - 1].Neurons[i].InputValue);
            for (int k = Layers.Count - 2; k > 0; k--)
                for (int i = 0; i < Layers[k].Neurons.Count; i++)
                {
                    differences[k][i] = 0;
                    for (int j = 0; j < Layers[k + 1].Neurons.Count; j++)
                        differences[k][i] += differences[k + 1][j] * Layers[k+1].Neurons[j].Inputs[i].Weight;
                    differences[k][i] *= Functions.BipolarDifferential(Layers[k].Neurons[i].InputValue);
                }
        }

        private void ChangeWeights(List<double> outputs, int row)
        {
            CalculateDifferences(outputs, row);
            for (int k = Layers.Count - 1; k > 0; k--)
                for (int i = 0; i < Layers[k].Neurons.Count; i++)
                    for (int j = 0; j < Layers[k - 1].Neurons.Count; j++)
                        Layers[k].Neurons[i].Inputs[j].Weight += 
                            LearningRate * 2 * differences[k][i] * Layers[k - 1].Neurons[j].OutputValue;
        }

        private void SaveWeights(string path)
        {
            List<string> tmp = ReadWeights();
            File.WriteAllLines(path, tmp);
        }

        private void LoadWeights(string[] lines)
        {
            try
            {
                int i = 0;
                foreach (Layer layer in Layers)
                    foreach (Neuron neuron in layer.Neurons)
                        foreach (Synapse synapse in neuron.Inputs)
                            synapse.Weight = Double.Parse(lines[i++]);
            }
            catch (Exception) { Console.WriteLine(" Incorrect input file"); }

        }

        private List<string> ReadWeights()
        {
            List<string> tmp = new List<string>();
            foreach (Layer layer in Layers)
                foreach (Neuron neuron in layer.Neurons)
                    foreach (Synapse synapse in neuron.Inputs)
                        tmp.Add(synapse.Weight.ToString());
            return tmp;
        }
    }
}