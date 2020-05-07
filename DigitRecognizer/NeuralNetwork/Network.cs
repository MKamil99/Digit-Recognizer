using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralNetwork
{
    class Network
    {
        static readonly double LearningRate = 0.05;
        double SynapsesCount = 0;
        internal List<Layer> Layers;
        internal double[][] ExpectedResults;
        double[][] ErrorFunctionChanges;

        public Network(int inputneuronscount, int hiddenlayerscount, int hiddenneuronscount, int outputneuronscount)
        {
            if (inputneuronscount < 1 || hiddenlayerscount > 0 && hiddenneuronscount < 1 || outputneuronscount < 1)
                throw new Exception("Incorrect Network Parameters");

            Layers = new List<Layer>();
            AddFirstLayer(inputneuronscount);
            for (int i = 0; i < hiddenlayerscount; i++) 
                AddNextLayer(new Layer(hiddenneuronscount));
            AddNextLayer(new Layer(outputneuronscount));

            SynapsesCount = CountSynapses();

            ErrorFunctionChanges = new double[Layers.Count][];
            for (int i = 1; i < Layers.Count; i++) 
                ErrorFunctionChanges[i] = new double[Layers[i].Neurons.Count];
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

            ExpectedResults = expectedvalues;
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

        public void Train(double[][][] datasets, double epochscount, bool showinfo = false, bool breaking = false)
        {
            double[][] trainingInputs = datasets[0], trainingOutputs = datasets[1];
            double recenterror = double.MaxValue, minerror = double.MaxValue;

            PushExpectedValues(trainingOutputs);
            Console.WriteLine(" Training neural network...");
            for (int i = 0; i < epochscount; i++)
            {
                List<double> outputs = new List<double>();
                for (int j = 0; j < trainingInputs.Length; j++)
                {
                    PushInputValues(trainingInputs[j]);
                    outputs = GetOutput();
                    ChangeWeights(outputs, j);
                }

                recenterror = CalculateMeanSquareError(datasets[2], datasets[3], showinfo);
                if (breaking == true && minerror < recenterror) break;
                minerror = recenterror;
            }

            SaveWeights("weights.txt");
            Console.WriteLine(" Done!");
        }

        private double CalculateMeanSquareError(double[][] inputs, double[][] expectedoutputs, bool showerror = false)
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
            if (showerror == true) Console.WriteLine($" Average mean square error: {Math.Round(error, 5)}");
            return error;
        }

        private void ChangeWeights(List<double> outputs, int row) // z użyciem algorytmu wstecznej propagacji
        {
            CalculateErrorFunctionChanges(outputs, row);
            for (int k = Layers.Count - 1; k > 0; k--)
                for (int i = 0; i < Layers[k].Neurons.Count; i++)
                    for (int j = 0; j < Layers[k - 1].Neurons.Count; j++)
                        Layers[k].Neurons[i].Inputs[j].Weight += 
                            LearningRate * 2 * ErrorFunctionChanges[k][i] * Layers[k - 1].Neurons[j].OutputValue;
        }

        private void CalculateErrorFunctionChanges(List<double> outputs, int row)
        {
            for (int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
                ErrorFunctionChanges[Layers.Count - 1][i] = (ExpectedResults[row][i] - outputs[i])
                    * Functions.BipolarDifferential(Layers[Layers.Count - 1].Neurons[i].InputValue);
            for (int k = Layers.Count - 2; k > 0; k--)
                for (int i = 0; i < Layers[k].Neurons.Count; i++)
                {
                    ErrorFunctionChanges[k][i] = 0;
                    for (int j = 0; j < Layers[k + 1].Neurons.Count; j++)
                        ErrorFunctionChanges[k][i] += ErrorFunctionChanges[k + 1][j] * Layers[k + 1].Neurons[j].Inputs[i].Weight;
                    ErrorFunctionChanges[k][i] *= Functions.BipolarDifferential(Layers[k].Neurons[i].InputValue);
                }
        }

        private void SaveWeights(string path)
        {
            List<string> tmp = new List<string>();
            for (int i = 1; i < Layers.Count; i++)
                foreach (Neuron neuron in Layers[i].Neurons)
                    foreach (Synapse synapse in neuron.Inputs)
                        tmp.Add(synapse.Weight.ToString());
            File.WriteAllLines(path, tmp);
        }

        public string LoadWeights(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Length != SynapsesCount)
                    return "NIEPRAWIDŁOWY PLIK WEJŚCIOWY (ILOŚĆ LINII)!";
                else
                {
                    try
                    {
                        int i = 0;
                        for (int j = 1; j < Layers.Count; j++)
                            foreach (Neuron neuron in Layers[j].Neurons)
                                foreach (Synapse synapse in neuron.Inputs)
                                    synapse.Weight = Double.Parse(lines[i++]);
                    }
                    catch (Exception) { return "NIEPRAWIDŁOWY PLIK WEJŚCIOWY (SPOSÓB ZAPISU)!"; }
                    return "SIEĆ JEST GOTOWA. ";
                }
            }
            else return "PLIK Z WAGAMI SYNAPS NIE ISTNIEJE!";
        }

        public string CalculatePrecision(double[][][] datasets) // z użyciem zbioru walidacyjnego
        {
            double[][] testingInputs = datasets[2], testingOutputs = datasets[3];
            List<double> outputs; int correct = 0;
            for (int i = 0; i < testingInputs.Length; i++)
            {
                PushInputValues(testingInputs[i]);
                outputs = GetOutput();
                if (outputs.IndexOf(outputs.Max()) == testingOutputs[i].ToList().IndexOf(1)) correct += 1;
            }
            double precision = Math.Round((double)correct / testingInputs.Length, 4) * 100;
            return $" DOKŁADNOŚĆ: {precision.ToString()}%";
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