using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class TestRunner
    {
        public static void RunTests(int[] T, int[] N, List<ITestFunction> testFunctions)
        {
            string reportFilePath = "report.csv";

            List<TestResult> testResults = new List<TestResult>();

            foreach (var testFunction in testFunctions)
            {
                foreach (var t in T)
                {
                    foreach (var n in N)
                    {
                        SnakeOptimization snakeOptimization = new SnakeOptimization(
                            _N: n,
                            _T: t,
                            _function: testFunction.Calculate,
                            _dim: testFunction.Dim,
                            _xmin: testFunction.Xmin,
                            _xmax: testFunction.Xmax
                        );

                        double[,] bestData = new double[testFunction.Dim + 1, 10];
                        int numberOfEvaluationFitnessFunction = 0;
                        string executionTime = "";

                        for (int i = 0; i < 10; i++)
                        {
                            double fBest;

                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            fBest = snakeOptimization.Solve();
                            watch.Stop();

                            var elapsedMs = watch.ElapsedMilliseconds;
                            executionTime = elapsedMs.ToString();

                            for (int j = 0; j < testFunction.Dim; j++)
                            {
                                bestData[j, i] = snakeOptimization.XBest[j];
                            }

                            bestData[testFunction.Dim, i] = fBest;
                            numberOfEvaluationFitnessFunction = snakeOptimization.NumberOfEvaluationFitnessFunction;
                        }

                        double minFunction = bestData[testFunction.Dim, 0];
                        int minFunction_index = 0;
                        for (int i = 1; i < 10; i++)
                        {
                            if (bestData[testFunction.Dim, i] < minFunction)
                            {
                                minFunction = bestData[testFunction.Dim, i];
                                minFunction_index = i;
                            }
                        }

                        double[] allMinFunction = new double[10];

                        for (int i = 0; i < 10; i++)
                        {
                            allMinFunction[i] = bestData[testFunction.Dim, i];
                        }

                        double avgForFunction = allMinFunction.Average();
                        double sumForFunction = allMinFunction.Sum(x => Math.Pow(x - avgForFunction, 2));
                        double stdDevForFunction = Math.Sqrt(sumForFunction / 10);
                        double varCoeffForFunction = 0;
                        if (stdDevForFunction != 0)
                            varCoeffForFunction = (stdDevForFunction / avgForFunction) * 100;
                        else
                            varCoeffForFunction = 0;


                        double[] minParameters = new double[testFunction.Dim];
                        for (int i = 0; i < testFunction.Dim; i++)
                            minParameters[i] = bestData[i, minFunction_index];

                        string str_minParameters = "(" + string.Join("; ", minParameters) + ")";

                        double[] stdDevForParameters = new double[testFunction.Dim];
                        double[] varCoeffForParameters = new double[testFunction.Dim];

                        for (int i = 0; i < testFunction.Dim; i++)
                        {
                            double[] parameters = new double[10];
                            for (int j = 0; j < 10; j++)
                            {
                                parameters[j] = bestData[i, j];
                            }

                            double avg = parameters.Average();
                            double sum = parameters.Sum(x => Math.Pow(x - avg, 2));
                            double stdDev = Math.Sqrt(sum / 10);
                            double varCoeff = 0;
                            if (stdDev != 0)
                                varCoeff = (stdDev / avg) * 100;
                            else
                                varCoeff = 0;

                            stdDevForParameters[i] = stdDev;
                            varCoeffForParameters[i] = varCoeff;
                        }

                        string str_stdDevForParameters = "(" + string.Join("; ", stdDevForParameters) + ")";
                        string str_varCoeffForParameters = "(" + string.Join("; ", varCoeffForParameters) + ")";

                        TestResult testResult = new TestResult
                        {
                            Algorytm = snakeOptimization.Name,
                            FunkcjaTestowa = testFunction.Name,
                            LiczbaSzukanychParametrów = testFunction.Dim,
                            LiczbaIteracji = t,
                            RozmiarPopulacji = n,
                            ZnalezioneMinimum = str_minParameters,
                            OdchylenieStandardowePoszukiwanychParametrów = str_stdDevForParameters,
                            WartośćFunkcjiCelu = minFunction,
                            OdchylenieStandardoweWartościFunkcjiCelu = stdDevForFunction,
                            LiczbaWywołańFunkcjiCelu = numberOfEvaluationFitnessFunction,
                            CzasEgzekucji = executionTime
                        };

                        testResults.Add(testResult);
                    }
                }
            }

            // Zapisz CSV
            var config = new CsvConfiguration(new System.Globalization.CultureInfo("en-US"));
            using (var writer = new StreamWriter(reportFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(testResults);
            }
        }
    }
}

