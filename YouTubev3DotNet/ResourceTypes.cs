using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubev3DotNet
{
    /// <summary>
    /// A resource is a data entity with a unique identifier. This class encapsulates the different types of resources 
    /// that can interact with the API.
    /// For more information see: https://developers.google.com/youtube/v3/getting-started
    /// </summary>
    public static class ResourceTypes
    {
        public static string Activity = "activity";
        public static string Channel = "channel";
        public static string ChannelBanner = "channelBanner";
        public static string GuideCategory = "guideCategory";
        public static string Playlist = "playlist";
        public static string PlaylistItem = "playlistItem";
        public static string SearchItem = "search result";
        public static string Subscription = "subscription";
        public static string Thumbnail = "thumbnail";
        public static string Video = "video";
        public static string VideoCategory = "videoCategory";
    }
}
