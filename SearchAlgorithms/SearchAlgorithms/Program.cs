using System;


namespace SearchAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SearchAlgorithm <FileName> <Method (BFS or DFS)>.");
                Environment.Exit(1);
            }
            string fileName = args[0];
            string method = args[1].ToUpper();
            Console.WriteLine(fileName);



            Maze maze = new Maze();

            bool value = maze.ReadFromFile(fileName);
            if(value)
            {
                Console.WriteLine("Maze:");
                maze.Print();
                Console.WriteLine("Solving...");
                maze.Solve(method);
                Console.WriteLine($"Status Explored: {maze.FNumExplored}");
                Console.WriteLine("Solution:");
                maze.Print();
                maze.OutputImage(@"maze.png");
            }
        }
    }
}
