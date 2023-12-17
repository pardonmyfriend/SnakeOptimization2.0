using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class StateWriter : IStateWriter
    {
        public void SaveToFileStateOfAlgorithm(string path, AlgorithmState algorithmState)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine($"{algorithmState.IterationNumber}");
                    writer.WriteLine($"{algorithmState.NumberOfEvaluationFitnessFunction}");

                    for (int i = 0; i < algorithmState.Fitness.Length; i++)
                    {
                        writer.WriteLine($"{string.Join(" ", algorithmState.Population[i])} {algorithmState.Fitness[i]}");
                    }
                }

                //Console.WriteLine($"Plik {path} został pomyślnie zapisany.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Wystąpił błąd podczas zapisu do pliku: {ex.Message}");
            }
        }
    }
}
