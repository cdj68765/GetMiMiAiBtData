using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

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
        public List<ByteInfo> BtList = new List<ByteInfo>();
        public List<ByteInfo> ImgList = new List<ByteInfo>();
        public List<string[]> InfoList = new List<string[]>();
        readonly Regex GetCodeRegex = new Regex(@"[a-zA-Z0-9//-]+", RegexOptions.Compiled);
        private string Title;
        private string GetCode;
        private object Actress;
        private string studio;
        private string series;
        private string[] Category;
        private DateTime date;
        private string magnet;
        private Dictionary<string, string> Match = new Dictionary<string, string>()
        {
            {"番号","GetCode"},
            {"女優:","Actress"},
            {"出演:","Actress"},
            {"スタジオ","studio"},
            {"シリーズ","series"},
            {"カテゴリ","Category"},
            {"発売日","date"},
            {"販売日","date"},
            {"配信日","date"},
            {"特征","magnet"},
            {"驗證編號","magnet"},
            {"驗證全碼","magnet"},
            {"種子代碼","magnet"},
        };
        internal void ReadBt(NetConnect net, string innerText)
        {
            InfoList.Add(new[] {"bt", innerText});
            BtList.Add(new ByteInfo
            {
                OriInfo = innerText
                // Byteinfo = net.GetByte(new Uri(innerText)),
                // Byteinfo = net.GetByte(new Uri("http://www7.2kdown.com/link.php?ref=2E6cs8MBHc")),
            });
        }

        internal void ReadInfo(string innerText)
        {
           
            InfoList.Add(new[] {"text", innerText});
            innerText = innerText.Replace("\r\n", "").Replace("&nbsp;","");
            if (string.IsNullOrEmpty(Title)&& innerText!="\r\n")
            {
                Title = innerText;
                return;
            }
            switch (Match.SingleOrDefault(VARIABLE => innerText.Contains(VARIABLE.Key)).Value)
            {
                case "GetCode":
                    GetCode = GetCodeRegex.Match(innerText).Value;
                    break;
                case "Actress":
                    SplitString(ref innerText);
                    Actress = innerText;
                    break;
                case "studio":
                    SplitString(ref innerText);
                    studio = innerText;
                    break;
                case "series":
                    SplitString(ref innerText);
                    series = innerText;
                    break;
                case "Category":
                    SplitString(ref innerText);
                    Category = innerText.Split(new char[] { ',', ' ', '?' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "date":
                    SplitString(ref innerText);
                    date = DateTime.Parse(GetCodeRegex.Match(innerText).Value);
                    break;
                case "magnet":
                    SplitString(ref innerText);
                    magnet = $"magnet:?xt=urn:btih:{innerText.Replace(" ", "")}";
                    break;
                default:
                    if(!string.IsNullOrWhiteSpace(innerText))
                    Debug.WriteLine($"{innerText}");
                    break;
            }
        }

        static void SplitString(ref String Text)
        {
            if (Text.Contains(":"))
            {
                Text = Text.Split(':')[1];
            }else if (Text.Contains("："))
            {
                Text = Text.Split('：')[1];
            }
            else if (Text.Contains(" "))
            {
                Text = Text.Split(' ')[1];
               
            }
            else
            {
                Debug.WriteLine($"{Text}");
            }
        }

        internal void ReadImg(NetConnect net, string innerText)
        {
            InfoList.Add(new[] {"img", innerText});
            ImgList.Add(new ByteInfo
            {
                OriInfo = innerText,
                //Byteinfo = net.GetByteDirect(innerText)
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