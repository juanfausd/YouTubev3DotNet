using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubev3DotNet
{
    public class RadiusComparerResult
    {
        public List<SearchResult> SmallRadiusItems { get; set; }
        public List<SearchResult> BigRadiusItems { get; set; }

        public List<SearchResult> DifferentItems 
        {
            get
            {
                List<string> idsBigRadius = BigRadiusItems.Select(r => r.Id.VideoId).Distinct().ToList();
                return SmallRadiusItems.Where(r => !idsBigRadius.Contains(r.Id.VideoId)).ToList();
            }
        }

        public RadiusComparerResult()
        {
            SmallRadiusItems = new List<SearchResult>();
            BigRadiusItems = new List<SearchResult>();
        }
    }
}
