using System;
using Climb.Core;

namespace Core.TestConsole
{
    internal class Program
    {
        private static void Main()
        {
            while (true)
            {
                Console.WriteLine("P1 elo");
                var p1Elo = int.Parse(Console.ReadLine());

                Console.WriteLine("P2 elo");
                var p2Elo = int.Parse(Console.ReadLine());

                Console.WriteLine("Who won? (1/2)");
                var p1Won = Console.ReadLine() == "1";

                var newElo = PlayerScoreCalculator.CalculateElo(p1Elo, p2Elo, p1Won);

                Console.WriteLine("P1 elo: " + newElo.Item1);
                Console.WriteLine("P2 elo: " + newElo.Item2); 
            }
        }
    }
}
