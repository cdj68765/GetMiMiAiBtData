using BetterHttpClient;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetMiMiAiBtData
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var Web = new HttpClient(new Proxy("127.0.0.1:7070")) {Encoding = Encoding.Default};
            /*using (var St = new StreamWriter(new FileStream("Web.txt", FileMode.Create)))
            {
                St.Write(Web.Get(@"http://www.mimirrr.com/forumdisplay.php?fid=55&page=1"));
            }*/
            var HtmlDoc = new HtmlAgilityPack.HtmlDocument();
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
                    var Url = NameAndUrl.Attributes["href"].Value;
                    var date = temp.SelectSingleNode("//td[4]//span").InnerText;
                    
                    /*using (var St = new StreamWriter(new FileStream("Web2.txt", FileMode.Create)))
                    {
                        St.Write(Web.GetStringAsync($"{"http://www.mimirrr.com/"}{Url}").Result);
                    }*/
                }
            }
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
