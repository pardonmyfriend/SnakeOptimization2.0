namespace SnakeOptimization
{
    public interface ITestFunction
    {
        string Name { get; set; }
        int Dim { get; set; }
        double[] Xmin { get; set; }
        double[] Xmax { get; set; }
        double Calculate(params double[] x);
    }
}