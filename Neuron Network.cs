using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MNIST_NeuronNetwork
{
    class Neuron_Network
    {
        int input_nodes = 784;
        int hidden_nodes = 100;
        int hidden_layers = 2;
        int output_nodes = 10;
        double learning_rate = 1;
        public Random randomizer = new Random((int)DateTime.Now.Ticks);

        public Neuron[][] neuron_arrays;

        //количество узлов во входном, скрытом и выходном слое

        public Neuron_Network()
        {

            try
            { 
                if (new FileInfo("network_settings.txt").Length == 0)
                {
                    throw new ArgumentException();
                }

                using (var reader = new StreamReader("network_settings.txt"))
                {
                    string buffer;
                    string number = "";
                    string[] parameters = new string[4];
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
                        catch (ArgumentOutOfRangeException) { }
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
                                if (x == buffer.Length - 1)
                                {
                                    number = number + buffer.Substring(x, 1);
                                    parameters[y] = number;
                                }
                                number = number + buffer.Substring(x, 1);
                            }
                            // #PARAMETERS: 784; 0; 0; 10; 0,3
                            // Network parameters

                            input_nodes = Convert.ToInt32(parameters[0]);
                            hidden_nodes = Convert.ToInt32(parameters[1]);
                            hidden_layers = Convert.ToInt32(parameters[2]);
                            output_nodes = Convert.ToInt32(parameters[3]);

                            neuron_arrays = new Neuron[2 + Convert.ToInt32(parameters[2])][];
                            for (int x = 0; x < Convert.ToInt32(parameters[2]) + 2; x++)
                            {
                                if (x == 0)
                                {
                                    neuron_arrays[0] = new Neuron[Convert.ToInt32(parameters[0])];
                                }
                                else if (x == Convert.ToInt32(parameters[2]) + 1)
                                {
                                    neuron_arrays[1 + Convert.ToInt32(parameters[2])] = new Neuron[Convert.ToInt32(parameters[3])];

                                }
                                else
                                {
                                    neuron_arrays[x] = new Neuron[Convert.ToInt32(parameters[1])];
                                }
                                for (int n = 0; n < neuron_arrays[x].Length; n++)
                                {
                                    if (x != 0)
                                    {
                                        neuron_arrays[x][n] = new Neuron(neuron_arrays[x - 1].Length, randomizer, "unnamed");
                                    }
                                    else
                                    {
                                        neuron_arrays[x][n] = new Neuron(1, randomizer, "unnamed");
                                        neuron_arrays[x][n].function = "net";
                                    }
                                    neuron_arrays[x][n].ID = "Neuron[" + x + ", " + n + "]";
                                }
                            }
                        }
                        else if (buffer != "")
                        {
                            number = "";
                            string layer_ID = "";
                            string neuron_ID = "";
                            string name = "";
                            List<double> args = new List<double>();
                            int[] ID_positions = new int[5];
                            bool has_ID = false;
                            bool named = false;
                            for (int x = 0; x < buffer.Length; x++)
                            {
                                if (has_ID)
                                {
                                    if (buffer.Substring(x, 1) == ";" && has_ID)
                                    {
                                        x++;
                                        args.Add(Convert.ToDouble(number));
                                        number = "";

                                    }
                                    else if (x == buffer.Length - 1)
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
                                        ID_positions[0] = x + 1;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if (buffer.Substring(x, 1) == ",")
                                    {
                                        ID_positions[1] = x;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if (buffer.Substring(x, 1) == "'" && !named)
                                    {
                                        ID_positions[2] = x + 2;
                                        name += buffer.Substring(x, 1);
                                        named = true;
                                    }
                                    else if (buffer.Substring(x, 1) == "'" && named)
                                    {
                                        ID_positions[3] = x - 1;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if (buffer.Substring(x, 1) == "]")
                                    {
                                        ID_positions[4] = x - 1;
                                        name += buffer.Substring(x, 1);
                                    }
                                    else if (buffer.Substring(x, 1) == ":")
                                    {

                                        layer_ID = buffer.Substring(ID_positions[0], ID_positions[1] - ID_positions[0]);
                                        neuron_ID = buffer.Substring(ID_positions[1] + 1, ID_positions[4] - ID_positions[1]);
                                        neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].ID = name.Substring(0, ID_positions[4] + 2);
                                        neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].name = buffer.Substring(ID_positions[2], ID_positions[3] - ID_positions[2]);
                                        has_ID = true;
                                    }
                                    else
                                    {
                                        name += buffer.Substring(x, 1);
                                    }

                                }
                            }
                            name = "";
                            for (int x = 0; x < neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].input_weights.Length; x++)
                            {
                                neuron_arrays[Convert.ToInt32(layer_ID)][Convert.ToInt32(neuron_ID)].input_weights[x] = args[x];
                            }
                        }
                        line++;
                    }
                }
            }
            catch (Exception error)
            {
                if (error is System.IO.FileNotFoundException || error is System.FormatException || error is ArgumentException)
                {
                    neuron_arrays = new Neuron[2 + hidden_layers][];
                    for (int x = 0; x < Convert.ToInt32(hidden_layers) + 2; x++)
                    {
                        if (x == 0)
                        {
                            neuron_arrays[0] = new Neuron[input_nodes];
                            for (int n = 0; n < neuron_arrays[0].Length; n++)
                            {
                                if (x == 0)
                                {
                                    neuron_arrays[x][n] = new Neuron(1, randomizer, "unnamed");
                                }
                                else
                                {
                                    neuron_arrays[x][n] = new Neuron(neuron_arrays[x - 1].Length, randomizer, "unnamed");
                                }
                                neuron_arrays[x][n].ID = "Neuron[" + x + ", " + n + "]";
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
                            neuron_arrays[x][n] = new Neuron(neuron_arrays[x - 1].Length, randomizer, "unnamed");
                            neuron_arrays[x][n].ID = "Neuron[" + x + ", " + n + "]";
                        }
                    }
                    using (var writer = new StreamWriter("network_settings.txt"))
                    {
                        writer.WriteLine("#PARAMETERS: " + input_nodes + "; "+ hidden_nodes + "; " + hidden_layers + ";" + output_nodes);
                        for (int x = 0; x < neuron_arrays.Length; x++)
                        {
                            writer.WriteLine();
                            for (int y = 0; y < neuron_arrays[x].Length; y++)
                            {
                                writer.Write(neuron_arrays[x][y].ID + " ''" + neuron_arrays[x][y].name + "'': ");
                                for (int z = 0; z < neuron_arrays[x][y].input_weights.Length; z++)
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
                        writer.WriteLine("Parameters: input nodes; hidden nodes; hidden layers; output nodes; learning rate");
                        writer.WriteLine("This is idle settings file generated for sigmoid neural network.");
                        writer.WriteLine("Made by LazyPhilosopher");
                    }
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

            double[][] error_per_neuron = new double[neuron_arrays.Length][];
            double[] network_output = Query(input_data);

            for (int x = 0; x < neuron_arrays.Length; x++)
            {
                error_per_neuron[x] = new double[neuron_arrays[x].Length];
            }

            for (int by_layer = neuron_arrays.Length - 1; by_layer >= 0; by_layer--)
            {
                if (by_layer == neuron_arrays.Length - 1)
                {
                    for (int by_neuron = 0; by_neuron < neuron_arrays[by_layer].Length; by_neuron++)
                    {
                        error_per_neuron[by_layer][by_neuron] = (answer_data[by_neuron] - neuron_arrays[by_layer][by_neuron].output);
                    }
                }
                if (by_layer > 0)
                {

                    for (int neuron_upper = 0; neuron_upper < error_per_neuron[by_layer].Length; neuron_upper++)
                    {
                        for (int x = 0; x < neuron_arrays[by_layer][neuron_upper].input_weights.Length; x++)
                        {
                            error_per_neuron[by_layer - 1][x] += error_per_neuron[by_layer][neuron_upper] * neuron_arrays[by_layer][neuron_upper].input_weights[x];
                        }
                        // ошибка верхнего нейрона распространяется соразмерно весам на нейроны в соседнем снизу слое 

                    }
                }
            }



            for (int by_layer = 1; by_layer < neuron_arrays.Length; by_layer++)
            {
                for (int by_neuron = 0; by_neuron < neuron_arrays[by_layer].Length; by_neuron++)
                {
                    double X_sum = 0;
                    if (by_layer > 0)
                    {
                        for (int by_weight = 0; by_weight < neuron_arrays[by_layer][by_neuron].input_weights.Length; by_weight++)
                        {
                            X_sum += neuron_arrays[by_layer][by_neuron].input_weights[by_weight] * neuron_arrays[by_layer - 1][by_weight].output;
                        }
                        for (int by_weight = 0; by_weight < neuron_arrays[by_layer][by_neuron].input_weights.Length; by_weight++)
                        {
                            neuron_arrays[by_layer][by_neuron].input_weights[by_weight] += error_per_neuron[by_layer][by_neuron] * (1 / (1 + Math.Exp(-X_sum))) * (1 - (1 / (1 + Math.Exp(-X_sum)))) * neuron_arrays[by_layer - 1][by_weight].output * learn_index;
                        }
                    }
                    else
                    {
                        for (int by_weight = 0; by_weight < neuron_arrays[by_layer][by_neuron].input_weights.Length; by_weight++)
                        {
                            neuron_arrays[by_layer][by_neuron].input_weights[by_weight] += error_per_neuron[by_layer][by_neuron] * (1 / (1 + Math.Exp(-input_data[by_neuron]))) * (1 - (1 / (1 + Math.Exp(-input_data[by_neuron])))) * input_data[by_neuron] * learn_index;
                        }
                    }
                }
            }
        }

        ~Neuron_Network()
        {
            using (var writer = new StreamWriter("network_settings.txt"))
            {
                writer.WriteLine("#PARAMETERS: " + input_nodes + "; " + hidden_nodes + "; " + hidden_layers + ";   " + output_nodes);
                for (int x = 0; x < neuron_arrays.Length; x++)
                {
                    writer.WriteLine();
                    for (int y = 0; y < neuron_arrays[x].Length; y++)
                    {
                        writer.Write(neuron_arrays[x][y].ID + " ''" + neuron_arrays[x][y].name + "'': ");
                        for (int z = 0; z < neuron_arrays[x][y].input_weights.Length; z++)
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
                writer.WriteLine("Parameters: input nodes; hidden nodes; hidden layers; output nodes");
                writer.WriteLine("This is idle settings file generated for sigmoid neural network.");
                writer.WriteLine("Made by LazyPhilosopher");
            }
        }

        double RndEx()
        {
            return ((double)randomizer.Next(0, 99999)) / 100000 - 0.5;
        }
    }
}