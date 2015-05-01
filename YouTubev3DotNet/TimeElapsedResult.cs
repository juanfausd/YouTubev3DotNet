using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubev3DotNet
{
    public class TimeElapsedResult
    {
        public List<SearchResult> Results { get; set; }
        public List<long> ElapsedSearch { get; set; }
        public List<long> ElapsedVideo { get; set; }
        public List<long> ElapsedChannel { get; set; }

        public TimeElapsedResult()
        {
            List<long> ElapsedSearch = new List<long>();
            List<long> ElapsedVideo = new List<long>();
            List<long> ElapsedChannel = new List<long>();
        }
    }
}
