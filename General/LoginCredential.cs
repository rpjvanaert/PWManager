using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace General
{
    [Serializable]
    public struct LoginCredentials : ISerializable
    {
        private string place;
        private string username;
        private string password;
        public LoginCredentials(string place, string username, string password)
        {
            this.place = place;
            this.username = username;
            this.password = password;
        }

        public string Place
        {
            get { return this.place; }
        }

        public string Username
        {
            get { return this.username; }
        }

        public string Password
        {
            get { return this.password; }
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
            throw new NotImplementedException();
        }
    }
}
