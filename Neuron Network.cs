using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MNIST_network
{
    class Neuron_Network
    {
        int input_nodes = 784;
        int hidden_nodes = 0;
        int hidden_layers = 0;
        int output_nodes = 10;
        double learning_rate;
        Random randomizer = new Random();

        Neuron[][] neuron_arrays;

        //количество узлов во входном, скрытом и выходном слое

        public Neuron_Network()
        {

            try
            {
                using (var reader = new StreamReader("network_settings.txt"))
                {
                    string buffer;
                    string number = "";
                    string[] parameters = new string[5];
                    int line = 0;
                    
                    while ((buffer = reader.ReadLine()) != null)
                    {
                        try
                        {
                            if (buffer.Substring(0, 11) == "COMMENTARY:")
                            {
                                reader.Close();
                                break;
                            }
                        }
                        catch(ArgumentOutOfRangeException) { }
                        if (line == 0)
                        {
                            int y = 0;
                            for (int x = 12; x < buffer.Length; x++)
                            {
                                
                                if (buffer.Substring(x, 1) == ";")
                                {
                                    parameters[y] = number;
                                    number = "";
                                    x++;
                                    y++;
                                }
                                if(x == buffer.Length - 1)
                                {
                                    number = number + buffer.Substring(x, 1);
                                    parameters[y] = number;
                                }
                                number = number + buffer.Substring(x, 1);
                            }
                            // #PARAMETERS: 784; 0; 0; 10; 0,3
                            // Network parameters
                            neuron_arrays = new Neuron[2 + Convert.ToInt32(parameters[2])][];
                            for (int x = 0; x < Convert.ToInt32(parameters[2]) + 2; x++)
                            {
                                if (x == 0)
                                {
                                    neuron_arrays[0] = new Neuron[Convert.ToInt32(parameters[0])];
                                    for (int n = 0; n < neuron_arrays[0].Length; n++)
                                    {
                                        neuron_arrays[x][n] = new Neuron(1);
                                    }
                                    x++;
                                }
                                if (x == Convert.ToInt32(parameters[2]) + 1)
                                {
                                    neuron_arrays[1 + Convert.ToInt32(parameters[2])] = new Neuron[Convert.ToInt32(parameters[3])];
                                }
                                else
                                {
                                    neuron_arrays[x] = new Neuron[Convert.ToInt32(parameters[1])];
                                }
                                for (int n = 0; n < neuron_arrays[x].Length; n++)
                                {
                                    neuron_arrays[x][n] = new Neuron(neuron_arrays[x - 1].Length);
                                }
                            }
                        }
                        else if(buffer != "")
                        {
                            number = "";
                            string layer_ID = "";
                            string neuron_ID = "";
                            string name = "";
                            List<double> args = new List<double>();
                            int[] ID_positions = new int[3];
                            bool named = false;
                            for (int x = 0; x < buffer.Length; x++)
                            {
                                if(named)
                                {
                                    if (buffer.Substring(x, 1) == ";" && named)
                                    {
                                        x++;
                                        args.Add(Convert.ToDouble(number));
                                        number = "";

                                    }
                                    else if(x == buffer.Length - 1)
                                    {
                                        number += buffer.Substring(x, 1);
                                        args.Add(Convert.ToDouble(number));
                                    }
                                    else
                                    {
                                        number += buffer.Substring(x, 1);
                                    }
                                }
                                else
                                {
                                    if (buffer.Substring(x, 1) == "[")
                                    {
                                        ID_positions[0] = x+1;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if(buffer.Substring(x, 1) == ",")
                                    {
                                        ID_positions[1] = x;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if(buffer.Substring(x, 1) == "]")
                                    {
                                        ID_positions[2] = x-1;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if (buffer.Substring(x, 1) == ":")
                                    {
                                        layer_ID = buffer.Substring(ID_positions[0], ID_positions[1] - ID_positions[0]);
                                        neuron_ID = buffer.Substring(ID_positions[1]+1, ID_positions[2] - ID_positions[1]);
                                        neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].name = name;
                                        named = true;
                                    }
                                    else
                                    {
                                        name += buffer.Substring(x, 1);
                                    }
                                    
                                }
                            }
                            name =  "";
                            for(int x = 0; x < neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].input_weights.Length; x++)
                            {
                                neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].input_weights[x] = args[x]; 
                            }
                        }
                        line++;
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                neuron_arrays = new Neuron[2 + hidden_layers][];
                for (int x = 0; x < Convert.ToInt32(hidden_layers) + 2; x++)
                {
                    if (x == 0)
                    {
                        neuron_arrays[0] = new Neuron[input_nodes];
                        for (int n = 0; n < neuron_arrays[0].Length; n++)
                        {
                            if(x == 0)
                            {
                                neuron_arrays[x][n] = new Neuron(1);
                            }
                            else
                            {
                                neuron_arrays[x][n] = new Neuron(neuron_arrays[x-1].Length);
                            }
                            neuron_arrays[x][n].name = "Neuron[" + x + ", " + n + "]";
                        }
                        x++;
                    }
                    if (x == Convert.ToInt32(hidden_layers) + 1)
                    {
                        neuron_arrays[1 + Convert.ToInt32(hidden_layers)] = new Neuron[Convert.ToInt32(output_nodes)];
                    }
                    else
                    {
                        neuron_arrays[x] = new Neuron[Convert.ToInt32(hidden_nodes)];
                    }
                    for (int n = 0; n < neuron_arrays[x].Length; n++)
                    {
                        neuron_arrays[x][n] = new Neuron(neuron_arrays[x - 1].Length);
                        neuron_arrays[x][n].name = "Neuron[" + x + ", " + n + "]";
                    }
                }
                using (var writer = new StreamWriter("network_settings.txt"))
                {
                    writer.WriteLine("#PARAMETERS: " + input_nodes + "; " + hidden_layers + "; " + hidden_nodes + "; " + output_nodes);
                    for (int x = 0; x < neuron_arrays.Length; x++)
                    {
                        writer.WriteLine();
                        for (int y = 0; y < neuron_arrays[x].Length; y++)
                        {
                            writer.Write(neuron_arrays[x][y].name + ": ");
                            for(int z = 0; z < neuron_arrays[x][y].input_weights.Length; z++)
                            {
                                if (z < neuron_arrays[x][y].input_weights.Length - 1)
                                {
                                    writer.Write(neuron_arrays[x][y].input_weights[z] + "; ");
                                }
                                else
                                {
                                    writer.WriteLine(neuron_arrays[x][y].input_weights[z]);
                                }
                            }
                        }
                    }
                    writer.WriteLine("");
                    writer.WriteLine("COMMENTARY: This is end of network settings.");
                    writer.WriteLine("Parameters: input nodes; hidden layers; hidden nodes; output nodes; learning rate");
                    writer.WriteLine("This is idle settings file generated for sigmoid neural network.");
                    writer.WriteLine("Made by LazyPhilosopher");
                }
            }

        }
        public double[] Query(double[] input_data)
        {
            double[] tmp = new double[1];
            for (int x = 0; x < neuron_arrays.Length; x++)
            {
                for (int y = 0; y < neuron_arrays[x].Length; y++)
                {
                    if (x == 0)
                    {
                        tmp = new double[1];
                        tmp[0] = input_data[y];
                        neuron_arrays[x][y].output = neuron_arrays[x][y].Query(tmp);
                    }
                    // опрос первого слоя
                    else if (x == neuron_arrays.Length - 1)
                    {
                        tmp = new double[neuron_arrays[x - 1].Length];
                        for (int z = 0; z < neuron_arrays[x - 1].Length; z++)
                        {
                            tmp[z] = neuron_arrays[x - 1][z].output;
                        }

                        neuron_arrays[x][y].output = neuron_arrays[x][y].Query(tmp);
                        //Console.WriteLine(neuron_arrays[x][y].output);
                    }
                    // опрос последнего слоя
                    else
                    {
                        tmp = new double[neuron_arrays[x - 1].Length];
                        for (int z = 0; z < neuron_arrays[x - 1].Length; z++)
                        {
                            tmp[z] = neuron_arrays[x - 1][z].output;
                        }
                        neuron_arrays[x][y].output = neuron_arrays[x][y].Query(tmp);
                    }
                    // опрос остальных слоев
                }
            }
            tmp = new double[neuron_arrays[neuron_arrays.Length - 1].Length];
            for (int z = 0; z < neuron_arrays[neuron_arrays.Length - 1].Length; z++)
            {
                tmp[z] = neuron_arrays[neuron_arrays.Length - 1][z].output;
            }
            return tmp;
        }

        public void Train(double[] input_data, double[] answer_data, double learn_index)
        {
            double layer_output = 0;
            double[][] error_per_neuron = new double[neuron_arrays.Length][];
            double[] netowrk_output = Query(input_data);
            for (int per_output = 0; per_output < output_nodes; per_output++)
            {
                for (int by_layer = neuron_arrays.Length - 1; by_layer > 0; by_layer--)
                {
                    error_per_neuron[by_layer] = new double[neuron_arrays.Length];
                    for (int by_neuron = 0; by_neuron < neuron_arrays[by_layer].Length; by_neuron++)
                    {
                        error_per_neuron[by_layer] = new double[neuron_arrays[by_layer - 1].Length];

                        if (by_layer == neuron_arrays.Length - 1)
                        {
                            error_per_neuron[by_layer][by_neuron] = neuron_arrays[by_layer][by_neuron].output / layer_output * Math.Pow((answer_data[by_neuron] - netowrk_output[by_neuron]), 2);
                        }
                        else
                        {
                            for (int by_neuron_lower = 0; by_neuron_lower < neuron_arrays[by_layer - 1].Length; by_neuron_lower++)
                            {
                                layer_output = layer_output + neuron_arrays[by_layer - 1][by_neuron_lower].output;
                            }
                            // сумма выходов нейронов в слое
                            for (int by_neuron_lower = 0; by_neuron_lower < neuron_arrays[by_layer - 1].Length; by_neuron_lower++)
                            {
                                error_per_neuron[by_layer][by_neuron_lower - 1] = neuron_arrays[by_layer - 1][by_neuron_lower].output / layer_output * error_per_neuron[by_layer][by_layer];
                            }
                            //error_per_neuron[neuron_arrays.Length - by_layer - 1][by_neuron] = neuron_arrays[neuron_arrays.Length - by_layer - 1][by_neuron].output / layer_output * ;
                        }
                    }

                }
            }
        }

        double RndEx()
        {
            return ((double)randomizer.Next(0, 99999)) / 100000 - 0.5;
        }
    }
}

