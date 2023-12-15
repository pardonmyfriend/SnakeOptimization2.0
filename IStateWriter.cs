using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public interface IStateWriter
    {
        void SaveToFileStateOfAlgorithm(string path, AlgorithmState algorithmState);
    }
}
