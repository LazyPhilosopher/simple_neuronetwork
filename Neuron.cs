using System;
using System.Collections.Generic;
using System.Text;

namespace MNIST_network
{
    class Neuron
    {
        public string function;
        public string name;
        public double[] input_weights;
        public double[] output_weights; // used only in output neurons
        public double output;
        Random rand = new Random();

        public Neuron(int rel_cnt)
        {
            function = "sigmoid";
            name = "";
            input_weights = new double[rel_cnt];
            for(int x = 0; x < rel_cnt; x++)
            {
                input_weights[x] = rand.Next(0, 100000)/100000.0;
            }
        }
        public double Query(double[] neuron_input)
        {
            double x = 0;
            if (function == "sigmoid")
            {
                for (int y = 0; y < neuron_input.Length; y++)
                {
                    x = x + neuron_input[y] * input_weights[y];
                }
                output = 1 / (1 + Math.Exp(-x));
            }
            else
            {
                output = 0;
            }
            return output;
        }
    }
}

