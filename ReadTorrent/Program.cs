using BencodeNET.Parsing;
using BencodeNET.Torrents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadTorrent
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new BencodeParser();
             var torrent = parser.Parse<Torrent>(@"C:\Users\cdj68\AppData\Local\Packages\23282Kaedei.play_3x7xvdr0843cr\LocalCache\Roaming\dandanplay\Cache\Torrents\30728B6F705F95E6C1C745F34C8E23D47E34E9B6.torrent");
             var list = torrent.Trackers;

        }
    }
}
