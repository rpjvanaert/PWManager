using System;
using System.Collections.Generic;
using General;
using Newtonsoft.Json;

namespace PWManager
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<LoginCredentials> logins = new List<LoginCredentials>();
            logins.Add(new LoginCredentials("admin", "admin", "admin"));

            Console.WriteLine(JsonConvert.SerializeObject(logins));
        }
    }
}
