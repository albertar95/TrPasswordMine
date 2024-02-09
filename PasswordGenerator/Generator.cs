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
        public Generator(int Length, List<CharType> charTypes)
        {
            _Length = Length;
            _CharTypes = charTypes;
        }
        public IEnumerable<string> GeneratePassword()
        {
            double rowCount = GetRowCount();//9
            int rowLength = rowCount > int.MaxValue ? int.MaxValue : Convert.ToInt32(rowCount);//9

            string[,] passwordMatrix = new string[rowLength, _Length];//9,2

            string[] correspondingChars = GetChars().ToArray();//a,b,c
            var corresCount = correspondingChars.Count();//3

            int corresIndex = 0;
            prepareChunckFiles();
            for (int i = _Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < rowLength / Math.Pow(corresCount, i); j++)
                {
                    var period = Convert.ToInt32(Math.Pow(corresCount, i));
                    for (int x = 0; x < period; x++)
                    {
                        //passwordMatrix[(j * period) + x, i] = correspondingChars[corresIndex];
                        WriteInTempFile(correspondingChars[corresIndex],i);
                    }
                    corresIndex++;
                    if (corresIndex >= corresCount)
                        corresIndex = 0;
                }
            }
            return MatrixToRow(passwordMatrix);
        }
        private List<string> MatrixToRow(string[,] passwordMatrix)
        {
            List<string> result = new List<string>();
            string current = "";
            for (int i = 0; i < passwordMatrix.GetLength(0); i++)
            {
                current = "";
                for (int j = 0; j < passwordMatrix.GetLength(1); j++)
                {
                    current += passwordMatrix[i, j];
                }
                result.Add(current);
            }
            return result;
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
        private void prepareChunckFiles()
        {
            for (int i = 0; i < _Length; i++)
            {
                var path = Path.Combine(DestinationFilePath, $"chunk{i}.txt");
                if (File.Exists(path))
                    File.Delete(path);
                File.Create(path);
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            }
        }
        private void WriteInTempFile(string corresChar,int fileId)
        {
            File.AppendAllLines(Path.Combine(DestinationFilePath, $"chunk{fileId}.txt"), new List<string>() { corresChar });
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
