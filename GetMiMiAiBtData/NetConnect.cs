using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BetterHttpClient;
using SocksSharp;
using SocksSharp.Proxy;
using HttpClient = System.Net.Http.HttpClient;

namespace GetMiMiAiBtData
{
    internal class HtmlTextHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var contentType = response.Content.Headers.ContentType;
            contentType.CharSet = await GetCharSetAsync(response.Content);
            return response;
        }

        private async Task<string> GetCharSetAsync(HttpContent httpContent)
        {
            var charset = httpContent.Headers.ContentType.CharSet;
            if (!string.IsNullOrEmpty(charset))
                return charset;

            var content = await httpContent.ReadAsStringAsync();
            var match = Regex.Match(content, @"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase);
            if (!match.Success)
                return charset;

            return match.Groups["charset"].Value;
        }
    }

    public class NetConnect : IDisposable
    {
        private HttpClient client;

        public bool ConnectWithProxy = false;
        private bool FirstCheck = true;
        public ProxySettings ProxySettings = new ProxySettings {Host = "127.0.0.1", Port = 7070};
        public bool TryDirectFirst = true;

        public NetConnect()
        {
            client = new HttpClient(new HtmlTextHandler());
        }

        public void Dispose()
        {
            client.Dispose();
        }


        public Tuple<HttpResponseMessage, byte[]> GetByte(Uri Uri)
        {
            HttpResponseMessage GetPost = null;
            if (TryDirectFirst)
                try
                {
                    GetPost = client
                        .PostAsync(Uri.Scheme + Uri.SchemeDelimiter + Uri.Authority + "/load.php",
                            new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("ref", Uri.Query.Split('=')[1]),
                                new KeyValuePair<string, string>("submit ", "点击下载")
                            })).Result;
                }
                catch (Exception)
                {
                }
            if (ConnectWithProxy && GetPost == null)
                try
                {
                    GetPost = new HttpClient(new ProxyClientHandler<Socks5>(ProxySettings))
                        .PostAsync(Uri.Scheme + Uri.SchemeDelimiter + Uri.Authority + "/load.php",
                            new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("ref", Uri.Query.Split('=')[1]),
                                new KeyValuePair<string, string>("submit ", "点击下载")
                            })).Result;
                }
                catch (Exception e)
                {
                }
            return GetPost == null
                ? new Tuple<HttpResponseMessage, byte[]>(null, null)
                : new Tuple<HttpResponseMessage, byte[]>(GetPost, GetPost.Content.ReadAsByteArrayAsync().Result);
        }

        public Tuple<HttpResponseMessage, byte[]> GetByteDirect(string Uri)
        {
            HttpResponseMessage GetPost = null;
            if (TryDirectFirst)
                try
                {
                    GetPost = client.GetAsync(Uri).Result;
                }
                catch (Exception)
                {
                }
            if (ConnectWithProxy && GetPost == null)
                try
                {
                    GetPost = new HttpClient(new ProxyClientHandler<Socks5>(ProxySettings)).GetAsync(Uri).Result;
                }
                catch (Exception)
                {
                }
            return GetPost == null
                ? new Tuple<HttpResponseMessage, byte[]>(null, null)
                : new Tuple<HttpResponseMessage, byte[]>(GetPost, GetPost.Content.ReadAsByteArrayAsync().Result);
        }

        public string GetHtml(string Url)
        {
            var RetHtml = "";
            if (TryDirectFirst)
                try
                {
                    RetHtml = client.GetStringAsync(Url).Result;
                }
                catch (Exception)
                {
                    TryDirectFirst = false;
                }
            if (RetHtml != "")
                return RetHtml;
            if (ConnectWithProxy && FirstCheck)
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 1, 0);
                    client = new HttpClient(new ProxyClientHandler<Socks5>(ProxySettings));
                    RetHtml = client.GetStringAsync(Url).Result;
                }
                catch (Exception)
                {
                    FirstCheck = false;
                }
                if (!FirstCheck)
                    try
                    {
                        var Web = new BetterHttpClient.HttpClient(
                            new Proxy(ProxySettings.Host + ":" + ProxySettings.Port))
                        {
                            Encoding = Encoding.Default
                        };
                        RetHtml = Web.Get(Url);
                    }
                    catch (Exception)
                    {
                    }
            }

            return RetHtml;
        }
    }
}