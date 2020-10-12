using System;
using System.Collections.Generic;
using System.Text;

namespace General
{
    class User
    {
        private string username;
        private string password;
        private List<LoginCredentials> logins;

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

    }
}
