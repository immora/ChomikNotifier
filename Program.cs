using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChomikNotifier
{
    public class Program
    {
        static readonly string url = "http://chomikuj.pl";

        static readonly string fileName = "searches.txt";

        public static void Main(string[] args)
        {
            //todo: pobieranie listy ostatnio wyszukiwanych odcinków

            Console.WriteLine("Search for:");

            string episodeToSearch = Console.ReadLine();

            SendRequest(episodeToSearch + " 720p").Wait();
        }

        private static async Task SendRequest(string searchQuery)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(url);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("filename", searchQuery)
                    });

                    var response = await client.PostAsync("/action/SearchFiles", content);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();

                    if (stringResponse.Contains("Nie znaleziono pliku"))
                    {
                        SaveToFile(searchQuery);
                        System.Console.WriteLine("File not found :(");
                    }
                    else
                    {
                        EraseFromFile(searchQuery);
                        System.Console.WriteLine("Found!");
                    }

                }
                catch (HttpRequestException e)
                {
                    System.Console.WriteLine($"Error message: {e.Message}");
                }
            }
        }

        private static void SaveToFile(string searchQuery)
        {
            var lines = File.ReadAllLines(fileName);

            foreach (var line in lines)
            {
                if (line.Equals(searchQuery))
                {
                    return;
                }
            }

            File.AppendAllLines(fileName, new List<string>() { searchQuery });
        }

        private static void EraseFromFile(string searchQuery)
        {
            List<string> lines = new List<string>(File.ReadAllLines(fileName));

            foreach (var line in lines)
            {
                if (line.Equals(searchQuery))
                {
                    lines.Remove(line);

                    File.WriteAllLines(fileName, lines.ToArray());

                    return;
                }
            }
        }
    }
}
