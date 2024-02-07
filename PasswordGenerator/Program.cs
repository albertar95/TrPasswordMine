using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter password length");
            var len = int.Parse(Console.ReadLine());
            var generator = new Generator(len,new List<CharType>() {  CharType.test });
            var result = generator.GeneratePassword();
            Console.ReadKey();
        }
    }
}
