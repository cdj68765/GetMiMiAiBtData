using System;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;

namespace GetMiMiAiBtData
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            /*   using (var net = new NetConnect() { TryDirectFirst = true, ConnectWithProxy = true })
               {
                   var Data = net.GetHtml("http://www.mimirrr.com/forumdisplay.php?fid=55&page=1");
               }*/
            var net = new NetConnect
            {
                TryDirectFirst = true,
                ConnectWithProxy = true
            };
            var HtmlDoc = new HtmlDocument();
            HtmlDoc.LoadHtml(File.OpenText("Web.txt").ReadToEnd());
            var Form = HtmlNode.CreateNode(HtmlDoc.DocumentNode.SelectNodes(
                "//div[@class='spaceborder']")[2].OuterHtml);
            foreach (var item in Form.SelectNodes(
                "//table"))
            {
                var temp = HtmlNode.CreateNode(item.OuterHtml);
                var NameAndUrl = temp.SelectSingleNode("//td[3]//a[1]");
                var name = NameAndUrl.InnerText;
                if (name.Contains("最新BT合集"))
                {
                    var PageTemp = new PageData
                    {
                        name = NameAndUrl.InnerText,
                        Url = NameAndUrl.Attributes["href"].Value,
                        date = temp.SelectSingleNode("//td[4]//span").InnerText
                    };
                    var NewHtml = new HtmlDocument();
                    NewHtml.LoadHtml(File.ReadAllText("Web2.txt"));
                    var Form2 = HtmlNode.CreateNode(NewHtml.DocumentNode.SelectNodes(
                        "//div[@class='t_msgfont']")[0].OuterHtml);
                    var Temp = new AVData();
                    foreach (var Child in Form2.ChildNodes)
                    {
                        switch (Child.Name)
                        {
                            case "a":
                                Temp.ReadBt(net, Child.InnerText);
                                PageTemp.ItemList.Add(Temp);
                                Temp = new AVData();
                                break;
                            case "#text":
                                Temp.ReadInfo(Child.InnerText);
                                break;
                            case "br":
                                Temp.InfoList.Add(new[] {"br", Child.InnerText});
                                break;
                            case "img":
                                Temp.ReadImg(net, Child.Attributes["src"].Value);
                                break;
                            default:
                                Debug.WriteLine($"{Child.InnerText}");
                                break;
                        }
                    }
                }
            }
            var tr = HtmlNode.CreateNode(HtmlDoc.DocumentNode.SelectNodes("//a[@class='p_pages']")[0].InnerHtml)
                .InnerText.Replace(@"&nbsp;", "").Split('/');
            /* var Web = new System.Net.Http.HttpClient(new HtmlTextHandler());
             using (var St = new StreamWriter(new FileStream("Web.txt", FileMode.Create)))
             {
                 St.Write(Web.GetStringAsync(@"http://www.mimirrr.com/forumdisplay.php?fid=55&page=1").Result);
             }*/

            //  var content = Encoding.Default.GetString(Web.DownloadData());
            /* Application.EnableVisualStyles();
             Application.SetCompatibleTextRenderingDefault(false);
             Application.Run(new Form1());*/
        }
    }
}