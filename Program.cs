
// See https://aka.ms/new-console-template for more information
//entry point for the program
var watch = System.Diagnostics.Stopwatch.StartNew();
SnakeOptimization.Main.main();
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"\nOverall execution time: {elapsedMs} ms\n");

//display pc specs
//cores
Console.WriteLine($"Cores: {Environment.ProcessorCount}");
//ram in GB
Console.WriteLine($"RAM: {Environment.WorkingSet / 1024 / 1024 / 2} GB");
//os
Console.WriteLine($"OS: {Environment.OSVersion}");
//dotnet version
Console.WriteLine($"Dotnet version: {Environment.Version}");