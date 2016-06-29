using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        static Uri queryUrl = new Uri("http://a3.launcher.eu/");

        static int queryInterval = 5000;

        static int totalIterations = 0;
        static int totalMatches = 0;
        static Timer timer;

        static string conditionText = "dirtysouthgaming";


        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing.");

            timer = new Timer(new TimerCallback(HandleTimer), null, 0, queryInterval);

            Console.WriteLine(String.Format("Polling every {0} seconds.", (queryInterval / 1000)));
            Console.WriteLine("Press Enter when done:");
            Console.ReadLine();

            Console.WriteLine(String.Format("Results: matches [{0}], total queries [{1}], {2}%.", 
                totalMatches, totalIterations, (totalMatches / totalIterations * 100)));

            timer.Dispose();

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }


        private  async static Task<String> queryAddress(Uri url)
        {
            Console.WriteLine("Querying...");

            var client = new HttpClient();
            return await client.GetStringAsync(url);
        }


        private static IEnumerable<HtmlNode> getSearchNodes(String inputHtml)
        {
            var document = new HtmlDocument();
            document.Load(new MemoryStream(Encoding.UTF8.GetBytes(inputHtml)));

            return document.DocumentNode.Descendants("tr")
                            .Where(d => d.Attributes.Contains("class") &&
                                    d.Attributes["class"].Value.Contains("sponsor")
                            );
        }


        private static async void HandleTimer(Object stateInfo)
        {

            var response = await queryAddress(queryUrl);
            var nodes = getSearchNodes(response);

            foreach(var node in nodes)
                if(node.InnerText.ToLower().Contains(conditionText))
                {
                    Console.WriteLine("Is match.");
                    totalMatches++; ;
                }

            totalIterations++;
        }
    }
}
