using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoom
{
    public class Messages
    {
        private string _messages;
        public string text { get; set; }
        public bool checker;

        public void SetPropertyContent()
        {
            Thread t = new Thread(fart);
            t.Start();
        }
        public void fart()
        {
                WebClient script = new WebClient();
                Task.Delay(2000);
                _messages = script.DownloadString("http://82.9.208.217:8080/");
                text = _messages;
        }
        public string username { get; set; }
    }
}
