using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    public class Generator
    {
        public int _Length { get; set; }
        public List<CharType> _CharTypes { get; set; }
        private List<string> numbers = new List<string>()
        { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private List<string> uppercases = new List<string>() { "A","B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R",
            "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private List<string> lowercases = new List<string>() { "a","b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
            "s", "t", "u", "v", "w", "x", "y", "z" };
        private List<string> specials = new List<string>() { ".","#", "$", "!", "%", "&", "@", "?", "=", ";", ":", "*", "~", "|", "_", "+",
            "^", ",","-", "{", "}", "(", ")", "[", "]", "<", ">" };
        private List<string> test = new List<string>() { "a","b","c" };
        public string DestinationFilePath { get; set; } = ConfigurationManager.AppSettings["DestinationFilePath"];
        public string ChunkSizeCount { get; set; } = ConfigurationManager.AppSettings["PasswordFileChunkCount"];
        private string FilenamePrefix;
        public Generator(int Length, List<CharType> charTypes)
        {
            _Length = Length;
            _CharTypes = charTypes;
            FilenamePrefix = $"passwords_{DateTime.Now.Ticks}_";
        }
        public void GeneratePassword()
        {
            double rowCount = GetRowCount();//9
            int rowLength = rowCount > int.MaxValue ? int.MaxValue : Convert.ToInt32(rowCount);//9

            string[,] passwordMatrix = new string[rowLength, _Length];//9,2

            string[] correspondingChars = GetChars().ToArray();//a,b,c
            var corresCount = correspondingChars.Count();//3

            int corresIndex = 0;
            //prepareChunckFiles();
            for (int i = _Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < rowLength / Math.Pow(corresCount, i); j++)
                {
                    var period = Convert.ToInt32(Math.Pow(corresCount, i));
                    for (int x = 0; x < period; x++)
                    {
                        passwordMatrix[(j * period) + x, i] = correspondingChars[corresIndex];
                        //WriteInTempFile(correspondingChars[corresIndex],i);
                    }
                    corresIndex++;
                    if (corresIndex >= corresCount)
                        corresIndex = 0;
                }
            }
            StreamWriteIntoFile(passwordMatrix);
        }
        public void GeneratePasswordPivot()
        {
            double rowCount = GetRowCount();//27
            long rowLength = rowCount > Int64.MaxValue ? Int64.MaxValue : Convert.ToInt64(rowCount);//27
            long[] passwordReputation = new long[_Length];
            string[] correspondingChars = GetChars().ToArray();//a,b,c
            var corresCount = correspondingChars.Count();//3
            long chunks = Convert.ToInt64(ChunkSizeCount);
            for (int i = 0; i < _Length; i++)
            {
                passwordReputation[i] = Convert.ToInt64(Math.Pow(corresCount,i));
            }
            string current = "";
            int index = 0;
            Console.WriteLine("password generation process started.please wait ...");
            if (rowLength > chunks)
            {
                for (int i = 0; i < rowLength / chunks; i++)
                {
                    for (int j = 0; j < chunks; j++)
                    {
                        current = "";
                        for (int k = 0; k < _Length; k++)
                        {
                            var rowIndex = (i * chunks) + j;
                            index = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(rowIndex) / Convert.ToDouble(passwordReputation[k]))) % corresCount;
                            current += correspondingChars[index];
                        }
                        WriteSinglePassword(current, i);
                    }
                    Console.WriteLine($"{((i+1)*chunks).ToString("#,#")} passwords created.please wait ...");
                }
            }else
            {
                for (int j = 0; j < rowLength; j++)
                {
                    current = "";
                    for (int k = 0; k < _Length; k++)
                    {
                        index = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(j) / Convert.ToDouble(passwordReputation[k]))) % corresCount;
                        current += correspondingChars[index];
                    }
                    WriteSinglePassword(current, 0);
                }
            }
            Console.WriteLine("all passwords generated successfully.Done!");
        }
        private void WriteSinglePassword(string pass,int chunk)
        {
            File.AppendAllLines(Path.Combine(DestinationFilePath,$"{FilenamePrefix}{chunk}.txt"),new List<string>() { pass });
        }
        private void StreamWriteIntoFile(string[,] passwordMatrix)
        {
            var path = Path.Combine(DestinationFilePath, "Generatedpasswords.txt");
            if (File.Exists(path))
                File.Delete(path);
            Console.WriteLine("password generator is processing.please wait ..." + Environment.NewLine);
            StreamWriter sw = new StreamWriter(path);
            //MatrixToRow(passwordMatrix);
            foreach (var pass in MatrixToRow(passwordMatrix))
            {
                sw.WriteLine(pass);
            }
            Console.WriteLine("all passwords generated and saved in file.Done!" + Environment.NewLine);
            sw.Close();
            sw.Dispose();
        }
        private IEnumerable<string> MatrixToRow(string[,] passwordMatrix)
        {
            //List<string> result = new List<string>();
            string current = "";
            for (int i = 0; i < passwordMatrix.GetLength(0); i++)
            {
                current = "";
                for (int j = 0; j < passwordMatrix.GetLength(1); j++)
                {
                    current += passwordMatrix[i, j];
                }
                //result.Add(current);
                yield return current;
            }
            //return result;
        }
        public double GetRowCount()
        {
            double rowCount = 0;
            foreach (var ct in _CharTypes)
            {
                if (_CharTypes.Contains(CharType.all))
                {
                    rowCount = numbers.Count + lowercases.Count + uppercases.Count + specials.Count;
                    break;
                }
                else
                {
                    switch (ct)
                    {
                        case CharType.numbers:
                            rowCount += numbers.Count;
                            break;
                        case CharType.lowercaseCharacters:
                            rowCount += lowercases.Count;
                            break;
                        case CharType.uppercaseCharacters:
                            rowCount += uppercases.Count;
                            break;
                        case CharType.specials:
                            rowCount += specials.Count;
                            break;
                        case CharType.test:
                            rowCount += test.Count;
                            break;
                    }
                }
            }
            return Math.Pow(rowCount,double.Parse(_Length.ToString()));
        }
        private List<string> GetChars()
        {
            var result = new List<string>();
            if(_CharTypes.Contains(CharType.all))
            {
                result.AddRange(numbers);
                result.AddRange(uppercases);
                result.AddRange(lowercases);
                result.AddRange(specials);
            }
            else
            {
                foreach (var ct in _CharTypes)
                {
                    switch (ct)
                    {
                        case CharType.numbers:
                            result.AddRange(numbers);
                            break;
                        case CharType.uppercaseCharacters:
                            result.AddRange(uppercases);
                            break;
                        case CharType.lowercaseCharacters:
                            result.AddRange(lowercases);
                            break;
                        case CharType.specials:
                            result.AddRange(specials);
                            break;
                        case CharType.test:
                            result.AddRange(test);
                            break;
                    }
                }
            }
            return result;
        }
        public int GetCharsCount()
        {
            int rowCount = 0;
            foreach (var ct in _CharTypes)
            {
                if (_CharTypes.Contains(CharType.all))
                {
                    rowCount = numbers.Count + lowercases.Count + uppercases.Count + specials.Count;
                    break;
                }
                else
                {
                    switch (ct)
                    {
                        case CharType.numbers:
                            rowCount += numbers.Count;
                            break;
                        case CharType.lowercaseCharacters:
                            rowCount += lowercases.Count;
                            break;
                        case CharType.uppercaseCharacters:
                            rowCount += uppercases.Count;
                            break;
                        case CharType.specials:
                            rowCount += specials.Count;
                            break;
                        case CharType.test:
                            rowCount += test.Count;
                            break;
                    }
                }
            }
            return rowCount;
        }
    }
    public enum CharType
    {
        all = 0,numbers = 1,uppercaseCharacters = 2, lowercaseCharacters = 3, specials = 4,test = 5
    }
    public class CorrespondingChars
    {
        public CharType Typo { get; set; }
        public string CharValue { get; set; }
    }
}
