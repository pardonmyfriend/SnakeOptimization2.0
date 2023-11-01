using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace SnakeOptimization
{
    public class Report
    {
        public static List<Result> createReport(List<Result> tableOfResults, double[,] data, int dim, string function, int T, int N, int funCallCounter, string executionTime)
        {
            double minFunction = data[dim, 0];
            int minFunction_index = 0;
            for (int i = 1; i < 10; i++)
            {
                if (data[dim, i] < minFunction)
                {
                    minFunction = data[dim, i];
                    minFunction_index = i;
                }
            }

            double[] allMinFunction = new double[10];

            for (int i = 0; i < 10; i++)
            {
                allMinFunction[i] = data[dim, i];
            }

            double avgForFunction = allMinFunction.Average();
            double sumForFunction = allMinFunction.Sum(x => Math.Pow(x - avgForFunction, 2));
            double stdDevForFunction = Math.Sqrt(sumForFunction / 10);
            double varCoeffForFunction = 0;
            if (stdDevForFunction != 0)
                varCoeffForFunction = (stdDevForFunction / avgForFunction) * 100;
            else
                varCoeffForFunction = 0;


            double[] minParameters = new double[dim];
            for (int i = 0; i < dim; i++)
                minParameters[i] = data[i, minFunction_index];

            string str_minParameters = "(" + string.Join("; ", minParameters) + ")";

            double[] stdDevForParameters = new double[dim];
            double[] varCoeffForParameters = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                double[] parameters = new double[10];
                for (int j = 0; j < 10; j++)
                {
                    parameters[j] = data[i, j];
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

            tableOfResults.Add(new Result
            {
                Algorytm = "SO",
                FunkcjaTestowa = function,
                LiczbaSzukanychParametrów = dim,
                LiczbaIteracji = T,
                RozmiarPopulacji = N,
                ZnalezioneMinimum = str_minParameters,
                OdchylenieStandardowePoszukiwanychParametrów = str_stdDevForParameters,
                WskaźnikZmiennościPoszukiwanychParametrów = str_varCoeffForParameters,
                WartośćFunkcjiCelu = minFunction.ToString(),
                OdchylenieStandardoweWartościFunkcjiCelu = stdDevForFunction.ToString(),
                WskaźnikZmiennościWartościFunkcjiCelu = varCoeffForFunction.ToString(),
                LiczbaWywołańFunkcjiCelu = funCallCounter.ToString(),
                CzasDziałania = executionTime
            });

            return tableOfResults;
        }

        public static void saveReport(List<Result> tableOfResults)
        {
            string reportFilePath = "raport.csv";

            try
            {
                var config = new CsvConfiguration(new System.Globalization.CultureInfo("en-US"));
                using (var writer = new StreamWriter(reportFilePath))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(tableOfResults);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}
