using System;
using System.Linq;
using OlimpoCache;
using TvChannelsParser;

namespace TvChannelParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new M3UTvChannelsPaser(new MemoryCacheProvider());
            parser.Load("/home/esqueleto/myShare/canais.m3u");

            parser.SaveChannels();

            // var groupChannels = parser.Channels
            //     .Select(x => x.GroupTitle)
            //     .Distinct()
            //     .ToList();

            // var noLogoChannels = parser.Channels
            //     .Where(x => 
            //         string.IsNullOrEmpty(x.Logo) &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");

            // var hdChannels = parser.Channels
            //     .Where(x => 
            //         x.ChannelQuality == ChannelQuality.HD &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");

            // var fhdChannels = parser.Channels
            //     .Where(x => 
            //         x.ChannelQuality == ChannelQuality.FHD &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");

            // var lowChannels = parser.Channels
            //     .Where(x => 
            //         x.ChannelQuality == ChannelQuality.Low &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");

            // var h265Channels = parser.Channels
            //     .Where(x => 
            //         x.ChannelQuality == ChannelQuality.H265 &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");

            // var normalChannels = parser.Channels
            //     .Where(x => 
            //         x.ChannelQuality == ChannelQuality.Normal &&
            //         x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");



            // var hdChannels = parser.Channels
            //     .Where(x => x.Name.Contains("[HD]"));

            // var fhdChannels = parser.Channels
            //     .Where(x => x.Name.Contains("[FHD]"));

            // var lowChannels = parser.Channels
            //     .Where(x => x.Name.Contains("[LOW]"));

            // var h265Channels = parser.Channels
            //     .Where(x => x.Name.Contains("[H265/HEVC]"));

            // Console.ReadLine();
        }
    }
}
