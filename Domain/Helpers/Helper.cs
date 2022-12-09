using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public static class Helper
    {
        public static string CleanKeywordResponse(string text)
        {
            var result = text.Trim()
                .Replace("\"", "")
                .Replace("/", "")
                .Replace(":", "")
                .Replace("Palavras-chave", "")
                .Replace("Palavras chave", "")
                .Replace("Palavra chave", "")
                .Replace(".", "");
            result = Regex.Replace(result, @"\r\n?|\n", "");
            return result;
        }

        public static List<string> SplitKeyword(string text)
        {
            List<string> keywords = new();
            List<string> formatedKeys = new();

            if (text.Contains(","))
            {
                keywords = text.Split(",").ToList();
                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim();
                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            }
            else if (text.Contains("-"))
            {
                keywords = text.Split("-").ToList();

                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim();
                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            }
            else if (text.Contains("1."))
            {
                keywords = text.Split(".").ToList();
                foreach (var item in keywords)
                {
                    var formatedItem = item.Trim()
                        .Replace("1", "")
                        .Replace("2", "")
                        .Replace("3", "")
                        .Replace("4", "")
                        .Replace("5", "");

                    if (!string.IsNullOrEmpty(formatedItem))
                        formatedKeys.Add(formatedItem);
                }
            }

            return formatedKeys;
        }

        public static List<string> ConfigureKeywords(List<string> keywords)
        {
            var configuredKeys = new List<string>();
            foreach (var item in keywords)
            {
                string key = item.Trim();
                if (string.IsNullOrEmpty(key)) continue;

                configuredKeys.Add($"(({item}))");
            }

            return configuredKeys;
        }
    }
}
