using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    //TODO: rename class name to more appropriate one
    public class Main
    {
        // properties
        public static int[] T = { 20, 40, 60, 80 };
        public static int[] N = { 20, 40, 60, 80, 100 };
        public static int dim = 2;

        //retries
        public static int n = 10;

        public static void main()
        {
            List<ITestFunction> testFunctions = new List<ITestFunction>();

            testFunctions.Add(new RastriginFunction(dim));
            testFunctions.Add(new SphereFunction(dim));

            IOptimizationAlgorithm optimizationAlgorithm = new SnakeOptimization();

            List<TestParam> testParams = new List<TestParam>();
            testParams.Add(new TestParam(80, 20, 20));
            testParams.Add(new TestParam(100, 20, 20));

            List<double[]> paramsList = new List<double[]>();

            foreach (var testParam in testParams)
            {
                int size = (int)((testParam.UpperBoundry -  testParam.LowerBoundry) / testParam.Step) + 1;

                double[] param = new double[size];

                int i = 0;

                for(var val = testParam.LowerBoundry; val <= testParam.UpperBoundry; val += testParam.Step)
                {
                    param[i] = val;
                    i++;
                }

                paramsList.Add(param);
            }

            TestRunner.RunTests(testFunctions, optimizationAlgorithm, paramsList);
        }
    }
}
