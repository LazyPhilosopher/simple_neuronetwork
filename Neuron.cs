using System;
using System.Collections.Generic;
using System.Text;

namespace MNIST_NeuronNetwork
{
    class Neuron
    {
        public string function;
        public string ID;
        public double[] input_weights;
        public double[] output_weights; // used only in output neurons
        public double[] last_input;
        public double output;
        public string name;

        public Neuron(int rel_cnt, Random rand, string neuron_name)
        {
            name = neuron_name;
            function = "sigmoid";
            ID = "";
            input_weights = new double[rel_cnt];
            for (int x = 0; x < rel_cnt; x++)
            {
                input_weights[x] = (rand.Next(0, 1000000000) - 500000000) / 1000000000.0;
            }
        }
        public double Query(double[] neuron_input)
        {
            last_input = new double[neuron_input.Length];
            last_input = neuron_input;
            double x = 0;

            for (int y = 0; y < neuron_input.Length; y++)
            {
                x = x + neuron_input[y] * input_weights[y];
            }

            if (function == "sigmoid")
            {
                output = 1 / (1 + Math.Exp(-x));
            }
            else if(function == "net")
            {
                output = x;
            }
            else
            {
                output = 0;
            }
            return output;
        }
    }
}
