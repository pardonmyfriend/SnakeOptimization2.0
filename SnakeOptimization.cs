using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf.parser;

namespace SnakeOptimization
{
    public delegate double fitnessFunction(params double[] x);

    class SnakeOptimization : IOptimizationAlgorithm
    {
        public string Name { get; set; }
        public ParamInfo[] ParamsInfo { get; set; }
        public IStateWriter writer { get; set; }
        public IStateReader reader { get; set; }
        public IGenerateTextReport stringReportGenerator { get; set; }
        public IGeneratePDFReport pdfReportGenerator { get; set; }
        public double[] XBest { get; set; }
        public double FBest { get; set; }
        public int NumberOfEvaluationFitnessFunction { get; set; }

        public SnakeOptimization()
        {
            Name = "Snake Optimization";

            ParamsInfo = new ParamInfo[]
            {
                new ParamInfo
                {
                    Name = "N",
                    Description = "Rozmiar populacji",
                    UpperBoundary = 100.0,
                    LowerBoundary = 10.0
                },
                new ParamInfo
                {
                    Name = "T",
                    Description = "Liczba iteracji",
                    UpperBoundary = 100.0,
                    LowerBoundary = 10.0
                }
            };

            writer = new StateWriter();
            reader = new StateReader();
            pdfReportGenerator = new GeneratePDFReport();
        }

        public void Solve(fitnessFunction f, double[,] domain, params double[] parameters)
        {
            int dim = domain.GetLength(1);

            XBest = new double[dim];
            FBest = 0.0;
            NumberOfEvaluationFitnessFunction = 0;

            double[] xmin = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                xmin[i] = domain[0, i];
            }

            double[] xmax = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                xmax[i] = domain[1, i];
            }

            int N = (int)parameters[0];
            int T = (int)parameters[1];
            int tStart = 1;

            Random rnd = new Random();

            double[][] X = new double[N][];
            double[] fitness = new double[N];

            if (File.Exists("state.txt"))
            {
                AlgorithmState algorithmState = reader.LoadFromFileStateOfAlgorithm("state.txt");

                tStart = algorithmState.IterationNumber + 1;
                NumberOfEvaluationFitnessFunction = algorithmState.NumberOfEvaluationFitnessFunction;
                X = algorithmState.Population;
                fitness = algorithmState.Fitness;
            }
            else
            {
                NumberOfEvaluationFitnessFunction = 0;

                // Initialize snake swarm and calculate fitness of each snake
                for (int i = 0; i < N; i++)
                {
                    X[i] = new double[dim];
                    for (int j = 0; j < dim; j++)
                    {
                        X[i][j] = xmin[j] + rnd.NextDouble() * (xmax[j] - xmin[j]);
                    }
                    fitness[i] = f(X[i]);
                    NumberOfEvaluationFitnessFunction++;
                }
            }

            // Constant variables
            double[] vecflag = { 1, -1 };
            double treshold1 = 0.25;
            double treshold2 = 0.6;
            double c1 = 0.5;
            double c2 = 0.05;
            double c3 = 2;

            // Get food position (Xfood)
            double bestSnake_fitValue = fitness.Min();
            int bestSnake_fitValue_index = Array.IndexOf(fitness, bestSnake_fitValue);
            XBest = X[bestSnake_fitValue_index].ToArray();

            // Divide the swarm
            int Nm = N / 2;
            int Nf = N - Nm;
            double[][] Xm = X.Take(Nm).ToArray(); // males
            double[][] Xf = X.Skip(Nm).ToArray(); // females
            double[] male_fitness = fitness.Take(Nm).ToArray();
            double[] female_fitness = fitness.Skip(Nm).ToArray();

            // Get best male
            double bestMale_fitValue = male_fitness.Min(); // get minimum value from fitness array
            int bestMale_fitValue_index = Array.IndexOf(male_fitness, bestMale_fitValue); // get index of this element
            double[] XBestMale = Xm[bestMale_fitValue_index].ToArray(); // find vector in the male matrix on this index

            // Get best female (same as male)
            double bestFemale_fitValue = female_fitness.Min();
            int bestFemale_fitValue_index = Array.IndexOf(female_fitness, bestFemale_fitValue);
            double[] XBestFemale = Xf[bestFemale_fitValue_index].ToArray();


            double[][] Xnewm = new double[Nm][];
            for (int i = 0; i < Nm; i++)
            {
                Xnewm[i] = new double[dim];
            }
            double[][] Xnewf = new double[Nf][];
            for (int i = 0; i < Nf; i++)
            {
                Xnewf[i] = new double[dim];
            }

            for (int t = tStart; t <= T; t++)
            {
                // Calculate temperature
                double Temp = Math.Exp(-(double)t / T);
                // Calculate food quantity
                double Q = c1 * Math.Exp(((double)t - T) / T);
                if (Q > 1)
                {
                    Q = 1;
                }

                if (Q < treshold1)
                {
                    // Exploration phase (no food)
                    // Every snake searches for food and goes to random position

                    // For males
                    for (int i = 0; i < Nm; i++)
                    {
                        int randmid = (int)(Nm * rnd.NextDouble());
                        double[] Xrandm = Xm[randmid].ToArray();
                        int flagid = (int)(2 * rnd.NextDouble());
                        double flag = vecflag[flagid];
                        double Am = Math.Exp(-male_fitness[randmid] / (male_fitness[i] + double.Epsilon));
                        for (int j = 0; j < dim; j++)
                        {
                            Xnewm[i][j] = Xrandm[j] + flag * c2 * Am * ((xmax[j] - xmin[j]) * rnd.NextDouble() + xmin[j]);
                        }
                    }

                    // For females
                    for (int i = 0; i < Nf; i++)
                    {
                        int randfid = (int)(Nf * rnd.NextDouble());
                        double[] Xrandf = Xf[randfid].ToArray();
                        int flagid = (int)(2 * rnd.NextDouble());
                        double flag = vecflag[flagid];
                        double Af = Math.Exp(-female_fitness[randfid] / (female_fitness[i] + double.Epsilon));
                        for (int j = 0; j < dim; j++)
                        {
                            Xnewf[i][j] = Xrandf[j] + flag * c2 * Af * ((xmax[j] - xmin[j]) * rnd.NextDouble() + xmin[j]);
                        }
                    }
                }
                else
                {
                    // Exploitation phase (food exists)
                    if (Temp > treshold2)
                    {
                        // Hot
                        // Snakes go to the food

                        // For males
                        for (int i = 0; i < Nm; i++)
                        {
                            int flagid = (int)(2 * rnd.NextDouble());
                            double flag = vecflag[flagid];
                            for (int j = 0; j < dim; j++)
                            {
                                Xnewm[i][j] = XBest[j] + flag * c3 * Temp * rnd.NextDouble() * (XBest[j] - Xm[i][j]);
                            }
                        }

                        // For females
                        for (int i = 0; i < Nf; i++)
                        {
                            int flagid = (int)(2 * rnd.NextDouble());
                            double flag = vecflag[flagid];
                            for (int j = 0; j < dim; j++)
                            {
                                Xnewf[i][j] = XBest[j] + flag * c3 * Temp * rnd.NextDouble() * (XBest[j] - Xf[i][j]);
                            }
                        }
                    }
                    else // Cold
                    {
                        if (rnd.NextDouble() > 0.6)
                        {
                            // Fight

                            // For males
                            for (int i = 0; i < Nm; i++)
                            {
                                double Fm = Math.Exp(-bestFemale_fitValue / (male_fitness[i] + double.Epsilon));
                                for (int j = 0; j < dim; j++)
                                {
                                    Xnewm[i][j] = Xm[i][j] + c3 * Fm * rnd.NextDouble() * (Q * XBestFemale[j] - Xm[i][j]);
                                }
                            }

                            // For females
                            for (int i = 0; i < Nf; i++)
                            {
                                double Ff = Math.Exp(-bestMale_fitValue / (female_fitness[i] + double.Epsilon));
                                for (int j = 0; j < dim; j++)
                                {
                                    Xnewf[i][j] = Xf[i][j] + c3 * Ff * rnd.NextDouble() * (Q * XBestMale[j] - Xf[i][j]);
                                }
                            }
                        }
                        else
                        {
                            // Mating

                            // For males
                            for (int i = 0; i < Nm; i++)
                            {
                                double Mm = Math.Exp(-female_fitness[i] / (male_fitness[i] + double.Epsilon));
                                for (int j = 0; j < dim; j++)
                                {
                                    Xnewm[i][j] = Xm[i][j] + c3 * Mm * rnd.NextDouble() * (Q * Xf[i][j] - Xm[i][j]);
                                }
                            }

                            // For females
                            for (int i = 0; i < Nf; i++)
                            {
                                double Mf = Math.Exp(-male_fitness[i] / (female_fitness[i] + double.Epsilon));
                                for (int j = 0; j < dim; j++)
                                {
                                    Xnewf[i][j] = Xf[i][j] + c3 * Mf * rnd.NextDouble() * (Q * Xm[i][j] - Xf[i][j]);
                                }
                            }

                            // Randomize if egg hatches
                            int flagid = (int)(2 * rnd.NextDouble());
                            double egg = vecflag[flagid];

                            // Check if egg is there or not
                            if (egg == 1)
                            {
                                // Get worst male and female
                                int worstMale_fitValue_index = Array.IndexOf(male_fitness, male_fitness.Max());
                                int worstFemale_fitValue_index = Array.IndexOf(female_fitness, female_fitness.Max());
                                // Replace them
                                for (int i = 0; i < dim; i++)
                                {
                                    Xnewm[worstMale_fitValue_index][i] = xmin[i] + rnd.NextDouble() * (xmax[i] - xmin[i]);
                                    Xnewf[worstFemale_fitValue_index][i] = xmin[i] + rnd.NextDouble() * (xmax[i] - xmin[i]);
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < Nm; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        if (Xnewm[i][j] > xmax[j])
                        {
                            Xnewm[i][j] = xmax[j];
                        }
                        if (Xnewm[i][j] < xmin[j])
                        {
                            Xnewm[i][j] = xmin[j];
                        }
                    }

                    double y = f(Xnewm[i]);
                    NumberOfEvaluationFitnessFunction++;
                    if (y < male_fitness[i])
                    {
                        male_fitness[i] = y;
                        Xm[i] = Xnewm[i].ToArray();
                    }
                }


                for (int i = 0; i < Nf; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        if (Xnewf[i][j] > xmax[j])
                        {
                            Xnewf[i][j] = xmax[j];
                        }
                        if (Xnewf[i][j] < xmin[j])
                        {
                            Xnewf[i][j] = xmin[j];
                        }
                    }

                    double y = f(Xnewf[i]);
                    NumberOfEvaluationFitnessFunction++;
                    if (y < female_fitness[i])
                    {
                        female_fitness[i] = y;
                        Xf[i] = Xnewf[i].ToArray();
                    }
                }

                double newBestMale_fitValue = male_fitness.Min();
                int newBestMale_fitValue_index = Array.IndexOf(male_fitness, newBestMale_fitValue);

                double newBestFemale_fitValue = female_fitness.Min();
                int newBestFemale_fitValue_index = Array.IndexOf(female_fitness, newBestFemale_fitValue);

                if (newBestMale_fitValue < bestMale_fitValue)
                {
                    XBestMale = Xm[newBestMale_fitValue_index].ToArray();
                    bestMale_fitValue = newBestMale_fitValue;
                }

                if (newBestFemale_fitValue < bestFemale_fitValue)
                {
                    XBestFemale = Xf[newBestFemale_fitValue_index].ToArray();
                    bestFemale_fitValue = newBestFemale_fitValue;
                }

                if (bestMale_fitValue < bestFemale_fitValue)
                {
                    FBest = bestMale_fitValue;
                    XBest = XBestMale.ToArray();
                }
                else
                {
                    FBest = bestFemale_fitValue;
                    XBest = XBestFemale.ToArray();
                }

                for (int i = 0; i < Nm; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        X[i][j] = Xm[i][j];
                    }
                }

                for (int i = 0; i < Nf; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        X[i + Nm][j] = Xm[i][j];
                    }
                }

                for (int i = 0; i < male_fitness.Length; i++)
                {
                    fitness[i] = male_fitness[i];
                }

                for (int i = 0; i < female_fitness.Length; i++)
                {
                    fitness[i + male_fitness.Length] = female_fitness[i];
                }

                AlgorithmState algorithmState = new AlgorithmState
                {
                    IterationNumber = t,
                    NumberOfEvaluationFitnessFunction = NumberOfEvaluationFitnessFunction,
                    Population = X,
                    Fitness = fitness
                };

                writer.SaveToFileStateOfAlgorithm("state.txt", algorithmState);

                string reportString = $@"Najlepszy osobnik: ({string.Join("; ", XBest)})
Wartosc funkcji celu dla najlepszego osobnika: {FBest}
Liczba wywolan funkcji celu: {NumberOfEvaluationFitnessFunction}
Liczba iteracji: {parameters[0]}
Rozmiar populacji: {parameters[1]}";

                stringReportGenerator = new GenerateTextReport(reportString);

                Console.WriteLine(stringReportGenerator.ReportString);

                string reportsFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                Directory.CreateDirectory(reportsFolder);
                //string path = System.IO.Path.Combine(reportsFolder, $"Report_{parameters[0]}_{parameters[1]}.pdf");
                string path = System.IO.Path.Combine(reportsFolder, $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                pdfReportGenerator.GenerateReport(path, reportString);

            }

            try
            {
                File.Delete("state.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas usuwania pliku: {ex.Message}");
            }
        }
    }
}
