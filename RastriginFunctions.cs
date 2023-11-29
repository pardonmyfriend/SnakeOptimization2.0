namespace SnakeOptimization
{
    public class RastriginFunction : ITestFunction
    {
        public string Name { get; set; }
        public int Dim { get; set; }
        public double[] Xmin { get; set; }
        public double[] Xmax { get; set; }

        public RastriginFunction(int _dim)
        {
            Name = "Funkcja Rastrigina";
            Dim = _dim;
            Xmin = Enumerable.Repeat(-5.12, _dim).ToArray();
            Xmax = Enumerable.Repeat(5.12, _dim).ToArray();
        }

        public double Calculate(params double[] X)
        {
            double sum = 0;
            for (int i = 0; i < X.Length; i++)
                sum += X[i] * X[i] - 10 * Math.Cos(2 * Math.PI * X[i]);

            return 10 * X.Length + sum;
        }
    }
}