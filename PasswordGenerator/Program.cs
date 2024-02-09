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
            var generator = new Generator(len,new List<CharType>() {  CharType.numbers });
            var result = generator.GeneratePassword();
            Console.ReadKey();
        }
        static void GatherInputs()
        {
            Console.WriteLine("==================== Password Generator ====================");
            PasswordLength:
            Console.WriteLine("enter password length");
            int passwordLength = 0;
            int charachtersOption = 0;
            if (!int.TryParse(Console.ReadLine(), out passwordLength))
            {
                Console.WriteLine("incorrect input");
                goto PasswordLength;
            }
            correspondingCharacters:
            Console.WriteLine("choose corresponding charachters" + Environment.NewLine);
            Console.WriteLine("1.All");
            Console.WriteLine("2.numbers");
            Console.WriteLine("3.lowercase charachters");
            Console.WriteLine("4.uppercase charachters");
            Console.WriteLine("5.special charachters");
            Console.WriteLine("6.numbers + lowercase charachters");
            Console.WriteLine("7.numbers + uppercase charachters");
            Console.WriteLine("8.numbers + special charachters");
            Console.WriteLine("9.lowercase charachters + uppercase charachters");
            Console.WriteLine("10.lowercase charachters + special charachters");
            Console.WriteLine("11.uppercase charachters + special charachters");
            Console.WriteLine("12.numbers + lowercase charachters + uppercase charachters");
            Console.WriteLine("13.numbers + uppercase charachters + special charachters");
            Console.WriteLine("14.numbers + lowercase charachters + special charachters");
            Console.WriteLine("15.lowercase charachters + uppercase charachters + special charachters");
            Console.WriteLine(Environment.NewLine);
            if (!int.TryParse(Console.ReadLine(), out charachtersOption))
            {
                Console.WriteLine("incorrect input");
                goto correspondingCharacters;
            }
            List<CharType> correspondings = new List<CharType>();
            switch (charachtersOption)
            {
                case 1:
                    correspondings.Add(CharType.all);
                    break;
                case 2:
                    correspondings.Add(CharType.numbers);
                    break;
                case 3:
                    correspondings.Add(CharType.lowercaseCharacters);
                    break;
                case 4:
                    correspondings.Add(CharType.uppercaseCharacters);
                    break;
                case 5:
                    correspondings.Add(CharType.specials);
                    break;
                case 6:
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.lowercaseCharacters);
                    break;
                case 7:
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.uppercaseCharacters);
                    break;
                case 8:
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.specials);
                    break;
                case 9:
                    correspondings.Add(CharType.lowercaseCharacters);
                    correspondings.Add(CharType.uppercaseCharacters);
                    break;
                case 10:
                    correspondings.Add(CharType.lowercaseCharacters);
                    correspondings.Add(CharType.specials);
                    break;
                case 11:
                    correspondings.Add(CharType.uppercaseCharacters);
                    correspondings.Add(CharType.specials);
                    break;
                case 12:
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.lowercaseCharacters);
                    correspondings.Add(CharType.uppercaseCharacters);
                    break;
                case 13:
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.uppercaseCharacters);
                    correspondings.Add(CharType.specials);
                    break;
                case 14:
                    correspondings.Add(CharType.lowercaseCharacters);
                    correspondings.Add(CharType.numbers);
                    correspondings.Add(CharType.specials);
                    break;
                case 15:
                    correspondings.Add(CharType.lowercaseCharacters);
                    correspondings.Add(CharType.uppercaseCharacters);
                    correspondings.Add(CharType.specials);
                    break;
            }
            ProcessInputs(passwordLength,correspondings);
            GatherInputs();
        }
        static void ProcessInputs(int length,List<CharType> chars)
        {
            Generator gen = new Generator(length,chars);
            var rowCount = gen.GetRowCount();
            var correspondingCount = gen.GetCharsCount();
            Console.WriteLine(Environment.NewLine + "----------------------------------------" + Environment.NewLine);
            Console.WriteLine($"you have choose {length} as password length and {correspondingCount} characters for generating passwords.");
            Console.WriteLine($"total count of passwords = {rowCount}");
            if(rowCount >= 2147483647)
            {
                Console.WriteLine("total count of password is too big and its not possible to continue");
                return;
            }
            Console.WriteLine($"generated password file size = {CalculateFileSize(rowCount,correspondingCount)}" + Environment.NewLine);
            fire:
            Console.WriteLine("do you want to start generating passwords? (y/n)");
            var fire = Console.ReadLine();
            switch (fire.ToLower())
            {
                case "y":
                    break;
                case "n":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("incorrect input");
                    goto fire;
            }


        }
        static string CalculateFileSize(double rowCount,int CorresCount)
        {
            var totalChars = rowCount * CorresCount;
            if (totalChars < 1024)
                return "1 kb";
            else if (totalChars > 1024 && totalChars < 1048576)//kb
                return $"{totalChars / 1024} kb";
            else if (totalChars > 1048576 && totalChars < 1073741824)//mb
                return $"{totalChars / 1048576} mb";
            else if (totalChars > 1073741824 && totalChars < 966367641600)//gb
                return $"{totalChars / 1073741824} gb";
            else
                return "more that 900 gb!";
        }
    }
}
