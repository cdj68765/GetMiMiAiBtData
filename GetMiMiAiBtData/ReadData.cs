using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GetMiMiAiBtData
{
    public class PageData
    {
        public List<AVData> ItemList = new List<AVData>();
        public string Url { get; internal set; }
        public string date { get; internal set; }
        public string name { get; internal set; }
    }

    public class AVData
    {
        public List<string[]> InfoList = new List<string[]>();
        public List<ByteInfo> BtList = new List<ByteInfo>();
        public List<ByteInfo> ImgList = new List<ByteInfo>();

        internal void ReadBt(NetConnect net, string innerText)
        {
            BtList.Add(new ByteInfo()
            {
                OriInfo = innerText,
                // Byteinfo = net.GetByte(new Uri(innerText)),
                // Byteinfo = net.GetByte(new Uri("http://www7.2kdown.com/link.php?ref=2E6cs8MBHc")),
            });

        }

        internal void ReadInfo(string innerText)
        {
        }

        internal void ReadImg(NetConnect net, string innerText)
        {
            ImgList.Add(new ByteInfo()
            {
                OriInfo = innerText,
               Byteinfo = net.GetByteDirect(innerText),
                 //Byteinfo = net.GetByteDirect("http://img588.net/images/2017/09/14/4.th.jpg"),
            });
        }

        public class ByteInfo
        {
            public byte[] _Byteinfo;

            public string OriInfo { get; internal set; }
            public string Status { get; set; }

            public Tuple<HttpResponseMessage, byte[]> Byteinfo
            {
                set
                {
                    if (value.Item1 == null)
                    {
                        Status = "Null";
                    }
                    else if (value.Item1.IsSuccessStatusCode)
                    {
                        Status = "GetOk";
                        _Byteinfo = value.Item2;
                    }
                    else
                    {
                        Status = value.Item1.ReasonPhrase;
                    }
                }
            }
        }
    }
}