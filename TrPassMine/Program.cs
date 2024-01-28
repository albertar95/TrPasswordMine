using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrPassMine
{
    class Program
    {
        static void Main(string[] args)
        {
            StartProgram();
        }
        static void StartProgram()
        {
            Console.WriteLine(Environment.NewLine + "start process? (y/n)");
            var input = Console.ReadLine().ToString().ToLower();
            switch (input)
            {
                case "y":
                    PassMine mine = new PassMine();
                    var result = mine.CheckPasswords().GetAwaiter().GetResult();
                    StartProgram();
                    break;
                case "n":
                    Environment.Exit(0);
                    break;
                default:
                    StartProgram();
                    break;
            }
        }
    }
}
