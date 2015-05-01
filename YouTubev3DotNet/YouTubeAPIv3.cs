using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace YouTubev3DotNet
{
    public class YouTubeAPIv3 : IYouTubeAPIv3
    {
        /// <summary>
        /// To get an API Key:
        /// - Go to https://console.developers.google.com
        /// - Create a new project (if you don't have one yet)
        /// - Go to APIs & Auth > Credentials
        /// - In Public API section access go to Create new Key
        /// - Create a Server Key
        /// - Replace the API_KEY constant with the value provided
        /// </summary>
        private const string API_KEY = "AIzaSyBmJqyqKdyNMNqd49L3f_Ojpapr1c07yuc";
        /// <summary>
        /// To get credentials related with OAuth 2.0:
        /// - Go to https://console.developers.google.com
        /// - Create a new project (if you don't have one yet)
        /// - Go to APIs & Auth > Credentials
        /// - In OAuth section access go to Create new Client ID
        /// - Create a service account
        /// - Copy the .p12 file generated to the bin folder
        /// - Replace the OAUTH_LOCAL_KEY_FILE constant with the current file name
        /// - Replace the OAUTH_PASSWORD constant with the value provided by google
        /// - Replace the OAUTH_SERVICE_ACCOUNT_EMAIL with the value provided by google
        /// </summary>
        private const string OAUTH_LOCAL_KEY_FILE = @"My Project-f991a24f5f25.p12";
        private const string OAUTH_PASSWORD = "notasecret";
        private const string OAUTH_SERVICE_ACCOUNT_EMAIL = "897412824056-lb1vrdp0d7dq8ocncchgp83doanrhtuf@developer.gserviceaccount.com";
        /// <summary>
        /// Some specific parameters to filter videos.
        /// </summary>
        private const int MAX_RESULTS_PER_PAGE = 50;
        private const string LOCATION = "40.7056308,-73.9780035";
        private const string LOCATION_RADIUS = "1000km";
        private const string PUBLISHED_FROM_DATE = "2014/09/15 16:37:02";
        /// <summary>
        /// Parameters to check different radius
        /// </summary>
        /// <param name="args"></param>
        private const int SMALL_RADIUS_FREQ_MINUTES = 1440;
        private const int BIG_RADIUS_FREQ_MINUTES = 60;

        /// <summary>
        /// Retrieves Videos Feeds from YouTube using API KEY.
        /// </summary>
        /// <returns></returns>
        public List<SearchResult> RetriveVideosUsingAPIKey(string searchTerm)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer());

            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
            listRequest.Fields = "items(id,snippet(title, description, publishedAt, thumbnails, channelId, channelTitle))";
            listRequest.Key = API_KEY;
            listRequest.Type = ResourceTypes.Video;
            listRequest.MaxResults = MAX_RESULTS_PER_PAGE;

            if (!string.IsNullOrEmpty(PUBLISHED_FROM_DATE))
                listRequest.PublishedAfter = DateTime.ParseExact(PUBLISHED_FROM_DATE, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(LOCATION))
                listRequest.Location = LOCATION;
            if (!string.IsNullOrEmpty(LOCATION_RADIUS))
                listRequest.LocationRadius = LOCATION_RADIUS;
            listRequest.Q = searchTerm;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            SearchListResponse searchResponse = listRequest.Execute();
            List<SearchResult> results = new List<SearchResult>();
            results.AddRange(searchResponse.Items);

            string nextPageToken = searchResponse.NextPageToken;

            int amountWithoutChannelTitle = 0;
            int amountWithoutChannelId = 0;

            while (searchResponse.Items.Count == MAX_RESULTS_PER_PAGE && !string.IsNullOrEmpty(nextPageToken))
            {
                foreach (var item in searchResponse.Items)
                {
                    if (string.IsNullOrEmpty(item.Snippet.ChannelTitle))
                        amountWithoutChannelTitle++;
                    if (string.IsNullOrEmpty(item.Snippet.ChannelId))
                        amountWithoutChannelId++;
                }
                listRequest.PageToken = nextPageToken;
                searchResponse = listRequest.Execute();
                results.AddRange(searchResponse.Items);
                nextPageToken = searchResponse.NextPageToken;
            }

            return results;
        }

        /// <summary>
        /// Retrieves Videos Feeds from YouTube using OAuth 2.0.
        /// </summary>
        /// <returns></returns>
        public List<SearchResult> RetriveVideosUsingOAuth(string searchTerm)
        {
            String serviceAccountEmail = OAUTH_SERVICE_ACCOUNT_EMAIL;

            var certificate = new X509Certificate2(OAUTH_LOCAL_KEY_FILE, OAUTH_PASSWORD, X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { YouTubeService.Scope.YoutubeReadonly }
               }.FromCertificate(certificate));

            // Create the service.
            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "YouTube API Sample",
            });

            SearchResource.ListRequest listRequest = service.Search.List("snippet");
            listRequest.Fields = "items(id,snippet(title, description, publishedAt, thumbnails, channelId, channelTitle))";
            listRequest.Type = ResourceTypes.Video;
            listRequest.MaxResults = MAX_RESULTS_PER_PAGE;
            if (!string.IsNullOrEmpty(PUBLISHED_FROM_DATE))
                listRequest.PublishedAfter = DateTime.ParseExact(PUBLISHED_FROM_DATE, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(LOCATION))
                listRequest.Location = LOCATION;
            if (string.IsNullOrEmpty(LOCATION_RADIUS))
                listRequest.LocationRadius = LOCATION_RADIUS;
            listRequest.Q = searchTerm;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            SearchListResponse searchResponse = listRequest.Execute();
            List<SearchResult> results = new List<SearchResult>();
            results.AddRange(searchResponse.Items);

            string nextPageToken = searchResponse.NextPageToken;

            while (searchResponse.Items.Count == MAX_RESULTS_PER_PAGE)
            {
                listRequest.PageToken = nextPageToken;
                searchResponse = listRequest.Execute();
                results.AddRange(searchResponse.Items);
                nextPageToken = searchResponse.NextPageToken;
            }

            return results;
        }

        /// <summary>
        /// Checks result differences between two radius.
        /// </summary>
        /// <param name="smallRadius"></param>
        /// <param name="bigRadius"></param>
        /// <returns></returns>
        public RadiusComparerResult CheckVideosDifferentRadius(string smallRadius, string bigRadius, string searchTerm)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer());

            // Performs search operation for the small radius
            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
            listRequest.Fields = "nextPageToken, prevPageToken, pageInfo, items(id,snippet(title, description, publishedAt, thumbnails, channelId, channelTitle))";
            listRequest.Key = API_KEY;
            listRequest.Type = ResourceTypes.Video;
            listRequest.MaxResults = MAX_RESULTS_PER_PAGE;
            if (!string.IsNullOrEmpty(LOCATION))
                listRequest.Location = LOCATION;
            if (!string.IsNullOrEmpty(LOCATION_RADIUS))
                listRequest.LocationRadius = smallRadius;
            listRequest.Q = searchTerm;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            // Splits the query in days
            DateTime dateFrom = DateTime.ParseExact(PUBLISHED_FROM_DATE, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            listRequest.PublishedAfter = dateFrom;
            listRequest.PublishedBefore = dateFrom.AddMinutes(SMALL_RADIUS_FREQ_MINUTES);

            SearchListResponse searchResponse = null;
            List<SearchResult> resultsSmallRadius = new List<SearchResult>();
            string nextPageToken = string.Empty;

            while (listRequest.PublishedAfter < DateTime.Today)
            {
                searchResponse = listRequest.Execute();
                resultsSmallRadius.AddRange(searchResponse.Items);
                nextPageToken = searchResponse.NextPageToken;

                while (searchResponse.Items.Count == MAX_RESULTS_PER_PAGE && !string.IsNullOrEmpty(nextPageToken))
                {
                    listRequest.PageToken = nextPageToken;
                    searchResponse = listRequest.Execute();
                    resultsSmallRadius.AddRange(searchResponse.Items);
                    nextPageToken = searchResponse.NextPageToken;
                }

                listRequest.PublishedAfter = listRequest.PublishedAfter.Value.AddMinutes(SMALL_RADIUS_FREQ_MINUTES);
                listRequest.PublishedBefore = listRequest.PublishedBefore.Value.AddMinutes(SMALL_RADIUS_FREQ_MINUTES);
                listRequest.PageToken = null;
            }

            // Performs search operation for the big radius
            listRequest.LocationRadius = bigRadius;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            // Splits the query in days
            listRequest.PublishedAfter = dateFrom;
            listRequest.PublishedBefore = dateFrom.AddMinutes(BIG_RADIUS_FREQ_MINUTES);

            List<SearchResult> resultsBigRadius = new List<SearchResult>();

            while (listRequest.PublishedAfter < DateTime.Today)
            {
                searchResponse = listRequest.Execute();
                resultsBigRadius.AddRange(searchResponse.Items);
                nextPageToken = searchResponse.NextPageToken;

                int totalItems = searchResponse.Items.Count;
                while (searchResponse.Items.Count == MAX_RESULTS_PER_PAGE && !string.IsNullOrEmpty(nextPageToken))
                {
                    listRequest.PageToken = nextPageToken;
                    searchResponse = listRequest.Execute();
                    resultsBigRadius.AddRange(searchResponse.Items);
                    nextPageToken = searchResponse.NextPageToken;
                    totalItems += searchResponse.Items.Count;
                }

                listRequest.PublishedAfter = listRequest.PublishedAfter.Value.AddMinutes(BIG_RADIUS_FREQ_MINUTES);
                listRequest.PublishedBefore = listRequest.PublishedBefore.Value.AddMinutes(BIG_RADIUS_FREQ_MINUTES);
                listRequest.PageToken = null;
            }

            // Checks for items that are in the small radius and aren't in big radius
            RadiusComparerResult result = new RadiusComparerResult();
            result.SmallRadiusItems = resultsSmallRadius;
            result.BigRadiusItems = resultsBigRadius;

            return result;
        }

        /// <summary>
        /// Retrieves a certain quantity of videos using oldest publish date criteria.
        /// </summary>
        /// <param name="maxItemsQuantity"></param>
        /// <returns></returns>
        public List<SearchResult> RetrieveVideosWithoutAPIPaging(int maxItemsQuantity, string searchTerm)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer());

            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
            listRequest.Fields = "items(id, snippet(title, description, publishedAt, thumbnails, channelId, channelTitle))";
            listRequest.Key = API_KEY;
            listRequest.Type = ResourceTypes.Video;
            listRequest.MaxResults = MAX_RESULTS_PER_PAGE;

            if (!string.IsNullOrEmpty(LOCATION))
                listRequest.Location = LOCATION;
            if (!string.IsNullOrEmpty(LOCATION_RADIUS))
                listRequest.LocationRadius = LOCATION_RADIUS;
            listRequest.Q = searchTerm;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            SearchListResponse searchResponse = listRequest.Execute();
            List<SearchResult> results = new List<SearchResult>();
            List<string> videosIds = new List<string>();
            List<string> channelsWithoutName = new List<string>();
            int amountWithoutChannelTitle = 0;
            int amountWithoutChannelId = 0;
            int currentCounter = 0;

            while (searchResponse.Items.Count > 0 && currentCounter < maxItemsQuantity)
            {
                foreach (var item in searchResponse.Items)
                {
                    videosIds.Add(item.Id.VideoId);
                    if (string.IsNullOrEmpty(item.Snippet.ChannelTitle))
                    {
                        channelsWithoutName.Add(item.Snippet.ChannelId);
                        amountWithoutChannelTitle++;
                    }
                    if (string.IsNullOrEmpty(item.Snippet.ChannelId))
                        amountWithoutChannelId++;
                }

                results.AddRange(searchResponse.Items);
                // Gets oldest element
                var oldest = searchResponse.Items.OrderBy(i => i.Snippet.PublishedAt).FirstOrDefault();
                // Avoids getting the oldest again
                listRequest.PublishedBefore = oldest.Snippet.PublishedAt.Value.AddSeconds(-1);
                currentCounter += searchResponse.Items.Count;
                if (currentCounter < maxItemsQuantity)
                {
                    // Performs the search
                    searchResponse = listRequest.Execute();
                }
            }

            // Retrieves videos recording details (location)
            VideosResource.ListRequest videosRequest = youtube.Videos.List("recordingDetails");
            videosRequest.Key = API_KEY;
            videosRequest.MaxResults = MAX_RESULTS_PER_PAGE;
            videosRequest.Id = string.Join(",", videosIds.Take(50).ToArray());
            VideoListResponse videosResponse = videosRequest.Execute();

            // Retrieves channels that don't have name
            List<string> channelsToRetrieve = channelsWithoutName.Take(50).ToList();
            channelsWithoutName = channelsWithoutName.Skip(50).ToList();
            while (channelsToRetrieve.Count > 0)
            {
                ChannelsResource.ListRequest channelRequest = youtube.Channels.List("snippet");
                channelRequest.Key = API_KEY;
                channelRequest.MaxResults = MAX_RESULTS_PER_PAGE;
                channelRequest.Id = string.Join(",", channelsToRetrieve.ToArray());
                ChannelListResponse channelsResponse = channelRequest.Execute();
                channelsToRetrieve = channelsWithoutName.Take(50).ToList();
                channelsWithoutName = channelsWithoutName.Skip(50).ToList();
            }

            return results;
        }

        /// <summary>
        /// Test the time elapsed in different API operations, in order to evaluate performance.
        /// </summary>
        /// <param name="maxItemsQuantity"></param>
        /// <returns></returns>
        public TimeElapsedResult TestOperationsTimeElapsed(int maxItemsQuantity, string searchTerm)
        {
            // Counters
            TimeElapsedResult result = new TimeElapsedResult();

            var youtube = new YouTubeService(new BaseClientService.Initializer());

            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
            listRequest.Fields = "items(id, snippet(title, description, publishedAt, thumbnails, channelId, channelTitle))";
            listRequest.Key = API_KEY;
            listRequest.Type = ResourceTypes.Video;
            listRequest.MaxResults = MAX_RESULTS_PER_PAGE;

            if (!string.IsNullOrEmpty(LOCATION))
                listRequest.Location = LOCATION;
            if (!string.IsNullOrEmpty(LOCATION_RADIUS))
                listRequest.LocationRadius = LOCATION_RADIUS;
            listRequest.Q = searchTerm;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            var stopwatch = Stopwatch.StartNew();
            SearchListResponse searchResponse = listRequest.Execute();
            result.ElapsedSearch.Add(stopwatch.ElapsedMilliseconds);
            List<SearchResult> results = new List<SearchResult>();
            List<string> videosIds = new List<string>();
            List<string> channelIds = new List<string>();
            int currentCounter = 0;

            while (searchResponse.Items.Count > 0 && currentCounter < maxItemsQuantity)
            {
                videosIds.AddRange(searchResponse.Items.Select(v => v.Id.VideoId));
                channelIds.AddRange(searchResponse.Items.Select(v => v.Snippet.ChannelId));
                results.AddRange(searchResponse.Items);
                // Gets oldest element
                var oldest = searchResponse.Items.OrderBy(i => i.Snippet.PublishedAt).FirstOrDefault();
                // Avoids getting the oldest again
                listRequest.PublishedBefore = oldest.Snippet.PublishedAt.Value.AddSeconds(-1);
                currentCounter += searchResponse.Items.Count;
                if (currentCounter < maxItemsQuantity)
                {
                    // Performs the search
                    stopwatch = Stopwatch.StartNew();
                    searchResponse = listRequest.Execute();
                    result.ElapsedSearch.Add(stopwatch.ElapsedMilliseconds);
                }
            }

            // Retrieves videos recording details (location)
            List<string> videosToRetrieve = videosIds.Take(50).ToList();
            videosIds = videosIds.Skip(50).ToList();
            while (videosToRetrieve.Count > 0)
            {
                VideosResource.ListRequest videosRequest = youtube.Videos.List("recordingDetails");
                videosRequest.Key = API_KEY;
                videosRequest.MaxResults = MAX_RESULTS_PER_PAGE;
                videosRequest.Id = string.Join(",", videosToRetrieve.ToArray());

                stopwatch = Stopwatch.StartNew();
                VideoListResponse videosResponse = videosRequest.Execute();
                result.ElapsedVideo.Add(stopwatch.ElapsedMilliseconds);

                videosToRetrieve = videosIds.Take(50).ToList();
                videosIds = videosIds.Skip(50).ToList();
            }

            // Retrieves channels
            List<string> channelsToRetrieve = channelIds.Take(50).ToList();
            channelIds = channelIds.Skip(50).ToList();
            while (channelsToRetrieve.Count > 0)
            {
                ChannelsResource.ListRequest channelRequest = youtube.Channels.List("snippet");
                channelRequest.Key = API_KEY;
                channelRequest.MaxResults = MAX_RESULTS_PER_PAGE;
                channelRequest.Id = string.Join(",", channelsToRetrieve.ToArray());

                stopwatch = Stopwatch.StartNew();
                ChannelListResponse channelsResponse = channelRequest.Execute();
                result.ElapsedChannel.Add(stopwatch.ElapsedMilliseconds);

                channelsToRetrieve = channelIds.Take(50).ToList();
                channelIds = channelIds.Skip(50).ToList();
            }

            result.Results = results;

            return result;
        }
    }
}
