using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public interface IOptimizationAlgorithm
    {
        string Name { get; set; }
        void Solve(fitnessFunction f, double[,] domain, params double[] parameters);
        ParamInfo[] ParamsInfo { get; set; }
        IStateWriter writer { get; set; }
        IStateReader reader { get; set; }
        double[] XBest { get; set; }
        double FBest { get; set; }
        int NumberOfEvaluationFitnessFunction { get; set; }
    }
}
