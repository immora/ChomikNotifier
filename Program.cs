using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChomikNotifier
{
    public class Program
    {
        static string url = "http://chomikuj.pl";
        
        public static void Main(string[] args)
        {
            //todo: pobieranie listy ostatnio wyszukiwanych odcinków
            
            StringBuilder sb = new StringBuilder();
            
            Console.WriteLine("Gimme tv show and episode number in format: TVSHOWNAME SxxExx");
                       
            string episodeToSearch = Console.ReadLine() + " 720p";
                       
            SendRequest(episodeToSearch).Wait();
        }
        
        private static async Task SendRequest(string fileName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(url);
                    
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("filename", fileName)
                    });
                    
                    var response = await client.PostAsync("/action/SearchFiles", content);
                    response.EnsureSuccessStatusCode();
                    
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    
                    
                    if (stringResponse.Contains("Nie znaleziono pliku"))
                    {
                        System.Console.WriteLine("Nie ma jeszcze odcinka :(");
                    }
                    else
                    {
                        System.Console.WriteLine("JEST ODCINEK!!!!!!!!! TODO: DORÓB ŚCIĄGANIE/OTWIERANIE STRONY/WYSYŁKĘ MAILA :P");
                    }
                    
                }
                catch(HttpRequestException e)
                {
                    System.Console.WriteLine($"Error message: {e.Message}");
                }
            }
        }
    }
}
