using System;
using OlimpoCache;
using TvChannelsParser;

namespace TvChannelParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new M3UTvChannelsPaser(new MemoryCacheProvider());
            Console.WriteLine($"Loading channel file: {args[0]}");
            parser.Load(args[0]);

            parser.SaveChannels();
        }
    }
}
