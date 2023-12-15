using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class StateReader : IStateReader
    {
        public AlgorithmState LoadFromFileStateOfAlgorithm(string path)
        {
            List<double[]> xList = new List<double[]>();
            List<double> yList = new List<double>();

            int iterationNumber = 0;
            int numberOfEvaluationFitnessFunction = 0;

            using (StreamReader reader = new StreamReader(path))
            {
                iterationNumber = int.Parse(reader.ReadLine());

                // Odczyt liczby wywołań funkcji celu
                numberOfEvaluationFitnessFunction = int.Parse(reader.ReadLine());

                // Odczyt populacji wraz z wartościami funkcji dopasowania
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(' ');

                    // Przykładowa obsługa danych - dostosuj do rzeczywistego formatu
                    double[] x = Array.ConvertAll(parts.Take(parts.Length - 1).ToArray(), double.Parse);
                    double y = double.Parse(parts.Last());

                    xList.Add(x);
                    yList.Add(y);
                }
            }

            double[][] X = xList.ToArray();
            double[] fitness = yList.ToArray();

            return new AlgorithmState
            {
                IterationNumber = iterationNumber,
                NumberOfEvaluationFitnessFunction = numberOfEvaluationFitnessFunction,
                Population = X,
                Fitness = fitness
            };
        }
    }
}
