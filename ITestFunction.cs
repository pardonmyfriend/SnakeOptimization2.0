namespace SnakeOptimization
{
    public interface ITestFunction
    {
        string Name { get; set; }
        int Dim { get; set; }
        double[,] Domain { get; set; }
        double Calculate(params double[] x);
    }
}