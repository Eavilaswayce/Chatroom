using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom
{
    class Extra : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
        public void PropertyChange(string PropertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Messages message;
        public string text
        {
            get => message.text;

            set
            {
                message.text = value;
                PropertyChanged(this, new PropertyChangedEventArgs("text"));
            }
        }
    }
}
