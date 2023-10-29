namespace SnakeOptimization
{
    delegate double Function(params double[] x);

    class SnakeOptimization : IOptimizationAlgorithm
    {
        private int N { get; set; }
        private int T { get; set; }
        private Function function { get; set; }
        private int dim { get; set; }
        private double[] xmin { get; set; }
        private double[] xmax { get; set; }

        public string Name { get; set; }
        public double[] XBest { get; set; }
        public double FBest { get; set; }
        public int NumberOfEvaluationFitnessFunction { get; set; }

        public SnakeOptimization(int _N, int _T, Function _function, int _dim, double[] _xmin, double[] _xmax)
        {
            this.N = _N;
            this.T = _T;
            this.function = _function;
            this.dim = _dim;
            this.xmin = _xmin;
            this.xmax = _xmax;

            this.XBest = new double[dim];
        }

        public (double[], double, int) Solve()
        {
            Random rnd = new Random();

            // searching for values
            double FBest = 0;

            // constant variables
            double[] vecflag = { 1, -1 };
            double treshold1 = 0.25;
            double treshold2 = 0.6;
            double c1 = 0.5;
            double c2 = 0.05;
            double c3 = 2;
            int funCallCounter = 0;

            double[][] X = new double[N][];
            double[] fitness = new double[N];

            // Initialize snake swarm and calculate fitness of each snake by objective function
            for (int i = 0; i < N; i++)
            {
                X[i] = new double[dim];
                for (int j = 0; j < dim; j++)
                {
                    X[i][j] = xmin[j] + rnd.NextDouble() * (xmax[j] - xmin[j]);
                }
                fitness[i] = function(X[i]);
                funCallCounter++;
            }

            // Get food position (Ffood)
            double bestSnake_fitValue = fitness.Min();
            int bestSnake_fitValue_index = Array.IndexOf(fitness, bestSnake_fitValue);
            double[] XBest = X[bestSnake_fitValue_index].ToArray();

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

            for (int t = 1; t <= T; t++)
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

                    double y = function(Xnewm[i]);
                    funCallCounter++;
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

                    double y = function(Xnewf[i]);
                    funCallCounter++;
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
            }


            return (XBest, FBest, funCallCounter);
        }
    }
}
