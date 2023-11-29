namespace SnakeOptimization
{
    public class Tools
    {
        /// <summary>
        /// Creates an array of size size and fills it with value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double[] Table(double value, int size)
        {
            double[] array = new double[size];
            for (int i = 0; i < size; i++)
                array[i] = value;
            return array;
        }

        public static (double, double) CoefOfVariation(double[] array)
        {
            double mean = 0;
            for (int i = 0; i < array.Length; i++)
                mean += array[i];
            mean /= array.Length;
            double stdDev = 0;
            for (int i = 0; i < array.Length; i++)
                stdDev += (array[i] - mean) * (array[i] - mean);
            stdDev /= array.Length;
            stdDev = Math.Sqrt(stdDev);
            if (stdDev != 0)
                return ((stdDev / mean) * 100, stdDev);
            else
                return (0, stdDev);

        }

    }

}