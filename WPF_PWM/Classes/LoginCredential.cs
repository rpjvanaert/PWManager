using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace General
{
    
    [Serializable]
    public class LoginCredentials : ISerializable, INotifyPropertyChanged
    {

        

        private string place;
        private string username;
        private string password;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginCredentials(string place, string username, string password)
        {
            this.place = place;
            this.username = username;
            this.password = password;
        }

        public string Place
        {
            get { return this.place; }
            set { this.place = value; }

        }

        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public override string ToString()
        {
            return place + " un: " + username + " pw: " + password;
        }

        public LoginCredentials(SerializationInfo info, StreamingContext context)
        {
            this.place = (string)info.GetValue("place", typeof(string));
            this.username = (string)info.GetValue("username", typeof(string));
            this.password = (string)info.GetValue("password", typeof(string));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("place", this.place, typeof(string));
            info.AddValue("username", this.username, typeof(string));
            info.AddValue("password", this.password, typeof(string));
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
