using System.Collections.Generic;

namespace TvChannelsParser
{
    public static class TvChannelExtentions
    {
        public static string ToJson(this IEnumerable<TvChannel> tvChannels)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(tvChannels);
        }
    }
}