using System.Net;
using static System.Console;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

namespace Crawler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Write("Enter number of words to return (default: 10): ");
            string? input = ReadLine();
            int.TryParse(input, out int numberOfWords);
            if (numberOfWords == 0)
            {
                numberOfWords = 10;
            }

            Write("Enter words to be excluded from th search (separate each word with a space): ");
            string? excludedWords = ReadLine();
            string[] wordsToExclude = excludedWords.Split(" ");
            Crowler(numberOfWords, wordsToExclude);           
        }

        
        public static void Crowler(int numberOfWords, string[] wordsToExclude)
        {
            string url = "https://en.wikipedia.org/wiki/Microsoft";            
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");            

            var wordDict = WordCount(htmlBody.InnerText, wordsToExclude, numberOfWords);

            Console.WriteLine();
            WriteLine(
              format: "{0,-10} {1,6}",
              arg0: "Word",
              arg1: "# of occurrences");

            Console.WriteLine();
            foreach (KeyValuePair<string, int> pair in wordDict)
            {                
                WriteLine(
                  format: "{0,-10} {1,6:N0}",
                  arg0: pair.Key,
                  arg1: pair.Value);
            }
        }

        public static Dictionary<string, int> WordCount(string contentText, string[] excluded, int numberOfWords)
        {
            var delimiterChars = new char[] { ' ', ',', ':', '\t', '\"', '\r', '{', '}', '[', ']', '=', '/' };
            var resultDict = new Dictionary<string, int>();
            if (excluded[0] == "")
            {
                resultDict = contentText
                    .Split(delimiterChars)
                    .Where(x => x.Length > 0)                    
                    .Where(x => !x.Contains('.'))
                    .GroupBy(x => x)
                    .Select(x => new { Word = x.Key, Count = x.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(numberOfWords)
                    .ToDictionary(x => x.Word, x => x.Count);
            }
            else
            {
                resultDict = contentText
                    .Split(delimiterChars)
                    .Where(x => x.Length > 0)
                    .Where(x => !excluded.Any(e => x.Contains(e)))
                    .Where(x => !x.Contains('.'))
                    .GroupBy(x => x)
                    .Select(x => new { Word = x.Key, Count = x.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(numberOfWords)
                    .ToDictionary(x => x.Word, x => x.Count);
            }
            return resultDict;
        }        
    }

}