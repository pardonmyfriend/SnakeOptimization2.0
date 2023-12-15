using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class AlgorithmState
    {
        public int IterationNumber { get; set; }
        public int NumberOfEvaluationFitnessFunction { get; set; }
        public double[][] Population { get; set; }
        public double[] Fitness { get; set; }
    }
}
