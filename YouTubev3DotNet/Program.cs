using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNetOpenAuth.OAuth2;

// To install the following dependencies, just run the following commands in the Package Manager Console:
// PM> Install-Package Google.Apis.Authentication -Pre
// PM> Install-Package Google.Apis.YouTube.v3
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Samples.Helper;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using System.Globalization;
using System.Diagnostics;

namespace YouTubev3DotNet
{
    /// <summary>
    /// This example provides an introduction to YouTubeAPI v3. In general, it shows a simple way to retrieve videos
    /// using both OAuth2.0 and API Key.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube v3 API - Service Account");
            Console.WriteLine("================================");
            Console.WriteLine(string.Empty);
            Console.WriteLine("1) Use API Key");
            Console.WriteLine("2) Use OAuth 2.0");
            Console.WriteLine("3) Compare 5km radius with 1000km radius");
            Console.WriteLine("4) Search videos without API paging");
            Console.WriteLine("5) Test operations time elapsed");
            Console.WriteLine(string.Empty);
            var option = CommandLine.RequestUserInput<string>("Option: ");
            if (string.IsNullOrEmpty(option) 
                && option != "1" 
                && option != "2"
                && option != "3"
                && option != "4"
                && option != "5")
            {
                Console.WriteLine("Invalid Option");
                CommandLine.PressAnyKeyToExit();
            }
            else if(option == "1")
            {
                Console.Clear();
                RetriveVideosUsingAPIKey();
            }
            else if (option == "2")
            {
                Console.Clear();
                RetriveVideosUsingOAuth();
            }
            else if (option == "3")
            {
                Console.Clear();
                CheckVideosDifferentRadius("5km", "1000km");
            }
            else if (option == "4")
            {
                Console.Clear();
                RetrieveVideosWithoutAPIPaging(50);
            }
            else if (option == "5")
            {
                Console.Clear();
                TestOperationsTimeElapsed(100);
            }
        }

        public static void RetriveVideosUsingAPIKey()
        {
            string searchTerm = CommandLine.RequestUserInput<string>("Search term: ");

            YouTubeAPIv3 api = new YouTubeAPIv3();
            List<SearchResult> results = api.RetriveVideosUsingAPIKey(searchTerm);

            CommandLine.WriteLine("\n");
            foreach (SearchResult searchResult in results)
            {
                CommandLine.WriteLine(searchResult.Snippet.Title.Replace("{", string.Empty).Replace("}", string.Empty) + " (" + searchResult.Id.VideoId + ")");
            }
            CommandLine.WriteLine(String.Format("Total:{0}", results.Count));

            CommandLine.PressAnyKeyToExit();
        }

        public static void RetriveVideosUsingOAuth()
        {
            string searchTerm = CommandLine.RequestUserInput<string>("Search term: ");

            YouTubeAPIv3 api = new YouTubeAPIv3();
            List<SearchResult> results = api.RetriveVideosUsingOAuth(searchTerm);

            CommandLine.WriteLine("\n");
            foreach (SearchResult searchResult in results)
            {
                CommandLine.WriteLine(searchResult.Snippet.Title.Replace("{", string.Empty).Replace("}", string.Empty) + " (" + searchResult.Id.VideoId + ")\n");
            }
            CommandLine.WriteLine(String.Format("Total:{0}", results.Count));

            CommandLine.PressAnyKeyToExit();
        }

        public static void CheckVideosDifferentRadius(string smallRadius, string bigRadius)
        {
            string searchTerm = CommandLine.RequestUserInput<string>("Search term: ");

            YouTubeAPIv3 api = new YouTubeAPIv3();
            RadiusComparerResult result = api.CheckVideosDifferentRadius(smallRadius, bigRadius, searchTerm);

            CommandLine.WriteLine("\n");
            CommandLine.WriteLine(String.Format("Total Small Radius: {0}", result.SmallRadiusItems.Count));
            CommandLine.WriteLine(String.Format("Total Big Radius: {0}", result.BigRadiusItems.Count));
            CommandLine.WriteLine(String.Format("Total Elements in Small Radius that aren't in Big Radius: {0}", result.DifferentItems.Count));

            CommandLine.PressAnyKeyToExit();
        }

        public static void RetrieveVideosWithoutAPIPaging(int maxItemsQuantity)
        {
            string searchTerm = CommandLine.RequestUserInput<string>("Search term: ");

            YouTubeAPIv3 api = new YouTubeAPIv3();
            List<SearchResult> results = api.RetrieveVideosWithoutAPIPaging(maxItemsQuantity, searchTerm);

            CommandLine.WriteLine("\n");
            foreach (SearchResult searchResult in results)
            {
                CommandLine.WriteLine(searchResult.Snippet.Title.Replace("{", string.Empty).Replace("}", string.Empty) + " (" + searchResult.Id.VideoId + ")");
            }
            CommandLine.WriteLine(String.Format("Total:{0}", results.Count));

            CommandLine.PressAnyKeyToExit();
        }

        public static void TestOperationsTimeElapsed(int maxItemsQuantity)
        {
            string searchTerm = CommandLine.RequestUserInput<string>("Search term: ");

            YouTubeAPIv3 api = new YouTubeAPIv3();
            TimeElapsedResult result = api.TestOperationsTimeElapsed(maxItemsQuantity, searchTerm);

            CommandLine.WriteLine("\n");

            // Shows elapsed times for search list operation
            CommandLine.WriteLine("Search List Operation");
            CommandLine.WriteLine("----------------");
            Console.WriteLine(string.Format("Average: {0}", result.ElapsedSearch.Average()));
            Console.WriteLine(string.Format("Max: {0}", result.ElapsedSearch.Max()));
            CommandLine.WriteLine("\n");

            // Shows elapsed times for video list operation
            CommandLine.WriteLine("Video List Operation");
            CommandLine.WriteLine("----------------");
            Console.WriteLine(string.Format("Average: {0}", result.ElapsedVideo.Average()));
            Console.WriteLine(string.Format("Max: {0}", result.ElapsedVideo.Max()));
            CommandLine.WriteLine("\n");

            // Shows elapsed times for channel list operation
            CommandLine.WriteLine("Channel List Operation");
            CommandLine.WriteLine("----------------");
            Console.WriteLine(string.Format("Average: {0}", result.ElapsedChannel.Average()));
            Console.WriteLine(string.Format("Max: {0}", result.ElapsedChannel.Max()));
            CommandLine.WriteLine("\n");

            CommandLine.WriteLine(String.Format("Total Videos:{0}", result.Results.Count));

            CommandLine.PressAnyKeyToExit();
        }
    }
}
