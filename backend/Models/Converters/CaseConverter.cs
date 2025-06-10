using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Hotel_MVP.Models.Converters
{
    public static class CaseConverter
    {
        public static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    result.Append(char.ToUpper(word[0]));
                    if (word.Length > 1)
                        result.Append(word.Substring(1).ToLower());
                }
            }

            return result.ToString();
        }

        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var pascalCase = ToPascalCase(str);
            return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
        }

        public static string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Regex.Replace(str, "([a-z])([A-Z])", "$1_$2").ToLower();
        }
    }
}