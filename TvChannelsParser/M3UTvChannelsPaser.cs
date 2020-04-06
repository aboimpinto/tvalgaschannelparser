using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using OlimpoCache;

namespace TvChannelsParser
{
    public class M3UTvChannelsPaser
    {
        private readonly List<TvChannel> _channels;
        private readonly List<string> _favorites;
        private TvChannel _currentTvChannel = null;
        private ICacheProvider _cacheProvider;
        private string _originalFile;

        public bool IsExtendend { get; set; }

        public string Checksum { get; set; }

        public ReadOnlyCollection<TvChannel> Channels => new ReadOnlyCollection<TvChannel>(this._channels);

        public M3UTvChannelsPaser(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;

            this._channels = new List<TvChannel>();

            this._favorites =  new List<string>
            {
                "RTP1",
                "RTP 1",
                "RTP 2",
                "SIC",
                "TVI",
                "SIC Noticias",
                "TVI 24",
                "RTP 3",
                "Sport TV 1",
                "SPORT TV 1",
                "Sport TV 2",
                "SPORT TV 2",
                "Sport TV 3",
                "SPORT TV 3",
                "Sport TV 4",
                "SPORT TV 4",
                "Sport TV 5",
                "SPORT TV 5",
                "BTV",
                "EUROSPORT 1",
                "EUROSPORT 2",
                "Eleven Sports 1",
                "ELEVEN SPORTS 1",
                "Eleven Sports 2",
                "ELEVEN SPORTS 2",
                "Eleven Sports 3",
                "ELEVEN SPORTS 3",
                "Eleven Sports 4",
                "ELEVEN SPORTS 4",
                "Eleven Sports 5",
                "ELEVEN SPORTS 5",
                "Eleven Sports 6",
                "ELEVEN SPORTS 6",
                "SPORT TV NBA",
                "NBA TV",
                "KOMBAT SPORT",
                "FIGHT NETWORK",
                "AXN WHITE",
                "AXN BLACK",
                "SYFY",
                "HISTORIA",
                "Discovery Channel",
                "Odisseia",
                "Travel Channel",
                "24 KITCHEN",
                "THE FOOD NETWORK",
                "TRAVEL CHANNEL",
            };
        }

        public void Load(string fileName)
        {
            this._originalFile = fileName;

            var md5 = MD5.Create();
            var checksum = md5.ComputeHash(File.ReadAllBytes(fileName));
            this.Checksum = BitConverter.ToString(checksum);

            using (var readerStream = File.OpenText(fileName))
            {
                var linesStream = readerStream
                    .ToObservableUntilEndOfStream()
                    .Subscribe(this.ParseM3UTvChannelsFile);
            }
        }

        public void SaveChannels()
        {
            this._cacheProvider.SetCacheValue(this.Checksum, this._channels.ToJson());

            // PORTUGAL Channels
            var unsortedPortugalChannels = this._channels
                .Where(x => x.GroupTitle == "PORTUGAL" || x.GroupTitle == "PORTUGAL [LOW]");
            this._cacheProvider.SetCacheValue($"{this.Checksum}_PORTUGAL", unsortedPortugalChannels.ToJson());

            var finalChannelList = new List<TvChannel>();
            foreach (var favoriteChannelName in this._favorites)
            {
                finalChannelList
                    .AddRange(unsortedPortugalChannels
                    .Where(x => 
                        x.Name == favoriteChannelName || 
                        x.Name.StartsWith($"{favoriteChannelName} [LOW]") || 
                        x.Name.StartsWith($"{favoriteChannelName} [FHD]") || 
                        x.Name.StartsWith($"{favoriteChannelName} [HD]") ||
                        x.Name.StartsWith($"{favoriteChannelName} [H265/HEVC]")));
            }

            var  targetFile = Path.Combine(Path.GetDirectoryName(this._originalFile), "sortedChannels.m3u");
            using (var writer = new StreamWriter(targetFile, false, Encoding.ASCII))
            {
                if (this.IsExtendend)
                {
                    writer.WriteLine("#EXTM3U");
                }

                foreach (var tvChannel in finalChannelList)
                {
                    writer.WriteLine($"#EXTINF:-1 tvg-id=\"{tvChannel.Id}\" tvg-name=\"{tvChannel.Name}\" tvg-logo=\"{tvChannel.Logo}\" group-title=\"\", {tvChannel.Name}");
                    writer.WriteLine($"{tvChannel.Path}|User-Agent=VLC");
                    // writer.WriteLine($"{tvChannel.Path}");
                }
            }

            this._cacheProvider.SetCacheValue("CurrentChannelList", this.Checksum);
        }

        private void ParseM3UTvChannelsFile(string m3uLine)
        {
            if (m3uLine == "#EXTM3U")
            {
                this.IsExtendend = true;
            }
            else if (m3uLine.StartsWith("#EXTINF"))
            {
                this._currentTvChannel = this.ExtractExtendedChannelInformation(m3uLine);
            }
            else
            {
                var m3uLineArray = m3uLine.Split('|');
                if (m3uLineArray.Length == 1)
                {
                    this._currentTvChannel.Path = m3uLine;
                }
                else
                {
                    this._currentTvChannel.Path = m3uLineArray[0].Trim();
                }
                
                this._channels.Add(this._currentTvChannel);
            }
        }

        private TvChannel ExtractExtendedChannelInformation(string line)
        {
            var auxInformation = line.Substring(11, line.Length - 11);

            var channel = new TvChannel();

            var tvgId_Index = auxInformation.IndexOf("tvg-id");
            var tvgName_Index = auxInformation.IndexOf("tvg-name");
            var tvgLogo_Index = auxInformation.IndexOf("tvg-logo");
            var groupTitle_Index = auxInformation.IndexOf("group-title");

            var tvgId = auxInformation.Substring(tvgId_Index, tvgId_Index + tvgName_Index).Trim();
            channel.Id = this.ExtractAndSanitizeValue(tvgId);

            var tvgName = auxInformation.Substring(tvgName_Index, tvgLogo_Index - tvgName_Index).Trim();
            var tvgNameArray = tvgName.Split(':');
            if (tvgNameArray.Length == 1)
            {
                channel.Name = this.ExtractAndSanitizeValue(tvgName);
            }
            else
            {
                channel.Name = tvgNameArray[0].Trim();
                channel.VideoParameters = tvgNameArray[1].Trim();
            }

            var tvgLogo = auxInformation.Substring(tvgLogo_Index, groupTitle_Index - tvgLogo_Index).Trim();
            channel.Logo = this.ExtractAndSanitizeValue(tvgLogo);

            var groupTitle = auxInformation.Substring(groupTitle_Index, auxInformation.Length - groupTitle_Index).Split(',')[0].Trim();
            channel.GroupTitle = this.ExtractAndSanitizeValue(groupTitle);

            return channel;
        }

        private string ExtractAndSanitizeValue(string value)
        {
            return value.Split('=')[1].Replace("\"", "");
        }
    }
}
