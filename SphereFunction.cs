using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnakeOptimization
{
    public class SphereFunction : ITestFunction
    {
        public string Name { get; set; }
        public int Dim { get; set; }
        public double[] Xmin { get; set; }
        public double[] Xmax { get; set; }

        public SphereFunction(int _dim)
        {
            Name = "Funkcja sferyczna";
            Dim = _dim;
            Xmin = Enumerable.Repeat(-5.0, _dim).ToArray();
            Xmax = Enumerable.Repeat(5.0, _dim).ToArray();
        }

        public double Calculate(params double[] X)
        {
            double sum = 0;
            for (int i = 0; i < X.Length; i++)
                sum += X[i] * X[i];

            return sum;
        }
    }
}
