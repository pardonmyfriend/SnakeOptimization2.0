using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnakeOptimization
{
    public class Main
    {
        public Main()
        {
            List<Result> tableOfResults = new List<Result>();

            List<Function> functions = new List<Function> { rastriginFunction, rosenbrockFunction, sphereFunction, bealeFunction, bukinFunctionN6, himmelblauFunctionN6 };
            int[] Ns = { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250 };
            int[] Ts = { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250 };
            int[] dims = { 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            foreach (Function function in functions)
            {
                double[] xmin;
                double[] xmax;
                double[] xmin2 = new double[2];
                double[] xmax2 = new double[2];

                if (function == rastriginFunction || function == rosenbrockFunction || function == sphereFunction || function == bealeFunction)
                {
                    foreach (int dim in dims)
                    {
                        if (function == rastriginFunction)
                        {
                            xmin = Enumerable.Repeat(-5.12, dim).ToArray();
                            xmax = Enumerable.Repeat(5.12, dim).ToArray();
                        }
                        else if (function == rosenbrockFunction)
                        {
                            xmin = Enumerable.Repeat(-5.0, dim).ToArray();
                            xmax = Enumerable.Repeat(5.0, dim).ToArray();
                        }
                        else if (function == sphereFunction)
                        {
                            xmin = Enumerable.Repeat(-5.0, dim).ToArray();
                            xmax = Enumerable.Repeat(5.0, dim).ToArray();
                        }
                        else
                        {
                            xmin = Enumerable.Repeat(-4.5, dim).ToArray();
                            xmax = Enumerable.Repeat(4.5, dim).ToArray();
                        }

                        foreach (int T in Ts)
                        {
                            foreach (int N in Ns)
                            {
                                SnakeOptimization snakeOptimization = new SnakeOptimization(N, T, function, dim, xmin, xmax);

                                double[,] data = new double[dim + 1, 10];
                                int funCallCounter = 0;

                                for (int i = 0; i < 10; i++)
                                {
                                    var result = snakeOptimization.Solve();

                                    for (int j = 0; j < dim; j++)
                                    {
                                        data[j, i] = result.Item1[j];
                                    }

                                    data[dim, i] = result.Item2;

                                    double[] XBest = result.Item1;
                                    double FBest = result.Item2;
                                    funCallCounter = result.Item3;

                                    Console.Write("XBest: {");
                                    foreach (int item in XBest)
                                    {
                                        Console.WriteLine(item + "");
                                    }
                                    Console.WriteLine("}");
                                    Console.WriteLine("FBest: " + FBest);
                                    Console.WriteLine("Liczba wywołań funkcji celu: " + funCallCounter);
                                }

                                tableOfResults.Add(new Result
                                {
                                    Algorytm = "SO",
                                    FunkcjaTestowa = nameof(function),
                                    LiczbaSzukanychParametrów = dim,
                                    LiczbaIteracji = T,
                                    RozmiarPopulacji = N,
                                    ZnalezioneMinimum = minimumParametres,
                                    StandartDeviationForParameters = StandartDeviationForParameters,
                                    VariationCoefficientForParameter = VariationCoefficientForParameter,
                                    ObjectiveFunction = data[D, minIndex].ToString(),
                                    StandartDeviationForFunction = StandartDeviationForFunction,
                                    VariationCoefficientForFunction = VariationCoefficientForFunction,
                                    Dimension = D,
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (function == bukinFunctionN6)
                    {
                        xmin2[0] = -15.0; xmin2[1] = -3.0;
                        xmax2[0] = -5.0; xmax2[1] = 3.0;
                    }
                    else
                    {
                        xmin2[0] = -5.0; xmin2[1] = -5.0;
                        xmax2[0] = 5.0; xmax2[1] = 5.0;
                    }

                    foreach (int N in Ns)
                    {
                        foreach (int T in Ts)
                        {
                            SnakeOptimization snakeOptimization = new SnakeOptimization(N, T, function, 2, xmin2, xmax2);

                            var result = snakeOptimization.Solve();

                            double[] array = result.Item1;
                            double value = result.Item2;
                            int number = result.Item3;

                            Console.WriteLine("Najlepszy snake: {");
                            foreach (int item in array)
                            {
                                Console.WriteLine(item);
                            }
                            Console.WriteLine("}");
                            Console.WriteLine("Wartość najlepszego snake: " + value);
                            Console.WriteLine("Liczba wykonanych funkcji celu: " + number);                           
                        }
                    }
                }
            }
        }
        public static double rastriginFunction(params double[] X)
        {

            double sum = 0;
            for (int i = 0; i < X.Length; i++)
                sum += X[i] * X[i] - 10 * Math.Cos(2 * Math.PI * X[i]);

            return 10 * X.Length + sum;

        }

        public static double rosenbrockFunction(params double[] X)
        {

            double sum = 0;
            for (int i = 0; i < X.Length - 1; i++)
                sum += 100 * Math.Pow(X[i + 1] - X[i] * X[i], 2) + Math.Pow(1 - X[i], 2);

            return sum;
        }

        public static double sphereFunction(params double[] X)
        {

            double sum = 0;
            for (int i = 0; i < X.Length; i++)
                sum += X[i] * X[i];

            return sum;
        }

        public static double bealeFunction(params double[] X)
        {
            double x = X[0];
            double y = X[1];

            return Math.Pow(1.5 - x + x * y, 2) + Math.Pow(2.25 - x + x * y * y, 2) + Math.Pow(2.625 - x + x * y * y * y, 2);
        }

        public static double bukinFunctionN6(params double[] X)
        {
            double x = X[0];
            double y = X[1];

            return 100 * Math.Sqrt(Math.Abs(y - 0.01 * x * x)) + 0.01 * Math.Abs(x + 10);
        }


        public static double himmelblauFunctionN6(params double[] X)
        {
            double x = X[0];
            double y = X[1];

            return Math.Pow(x * x + y - 11, 2) + Math.Pow(x + y * y - 7, 2);
        }

    }
}
