using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubev3DotNet
{
    public interface IYouTubeAPIv3
    {
        List<SearchResult> RetriveVideosUsingAPIKey(string searchTerm);
        List<SearchResult> RetriveVideosUsingOAuth(string searchTerm);
        RadiusComparerResult CheckVideosDifferentRadius(string smallRadius, string bigRadius, string searchTerm);
        List<SearchResult> RetrieveVideosWithoutAPIPaging(int maxItemsQuantity, string searchTerm);
        TimeElapsedResult TestOperationsTimeElapsed(int maxItemsQuantity, string searchTerm);
    }
}
