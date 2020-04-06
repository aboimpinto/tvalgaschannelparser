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
        }
    }
}
