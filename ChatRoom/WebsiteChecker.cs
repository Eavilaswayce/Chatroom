using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom
{
    public class WebsiteChecker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
        public void PropertyChange(string PropertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        private Messages _msg = new Messages();
        public void LoadAllData()
        {
            _msg.SetPropertyContent();
            msg = _msg;
        }
        public Messages msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
                PropertyChange("msg");
            }
        }

        public string flopper()
        {
            return _msg.text;
        }

        /*private string _text;
        public Messages text
        {
            get
            {
                WebClient script = new WebClient();
                string messages = script.DownloadString("http://82.9.208.217:8080/");
                _fart.text = messages;
                return _fart;
            }
            set
            {
                _fart = value;
                PropertyChange("text");
            }
        }*/
        public object Dispatcher { get; private set; }
    }
}
