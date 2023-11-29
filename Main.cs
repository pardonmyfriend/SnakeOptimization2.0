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
        public static int[] T = { 5, 10, 20, 40, 60, 80 };
        public static int[] N = { 10, 20, 40, 80 };
        public static int dim = 2;

        //retries
        public static int n = 10;

        public static void main()
        {
            List<ITestFunction> testFunctions = new List<ITestFunction>();

            testFunctions.Add(new RastriginFunction(dim));
            testFunctions.Add(new SphereFunction(dim));

            TestRunner.RunTests(T, N, testFunctions);
        }
    }
}
