using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PWManager
{
    class Program
    {
        public static void Main(string[] args)
        {
            Client client = Client.Instance;
            client.dosomething();
            //    List<LoginCredentials> logins = new List<LoginCredentials>();
            //    logins.Add(new LoginCredentials("pc", "admin", "admin"));
            //    logins.Add(new LoginCredentials("phone", "admin", "admin"));
            //    logins.Add(new LoginCredentials("mail", "admin", "admin"));
            //    logins.Add(new LoginCredentials("facebook", "admin", "admin"));

            //    dynamic json = new
            //    {
            //        data = logins.ToArray()
            //    };
            //    //Console.WriteLine(json);


            //    string message = JsonConvert.SerializeObject(json);
            //    JObject jObject = JObject.Parse(message);
            //    JArray jArray = (JArray)jObject["data"];
            //    Console.WriteLine(jArray);

            //    dynamic output = JsonConvert.DeserializeObject(message);

            //    try
            //    {
            //        //Console.WriteLine(output);
            //        foreach (dynamic d in output.data)
            //        {
            //            //Console.WriteLine("Place: " + d.place + ", Username: " + d.username + ", Password: " + d.password);
            //        }
            //    } catch
            //    {
            //        Console.WriteLine("Can't");

            
        }

        class Client
        {
            private static readonly Client instance = new Client();

            private Client() { }

            public static Client Instance
            {
                get 
                {
                    return instance;
                }
            }

            public void dosomething() 
            {
                dynamic jsonDyn = new
                {
                    data = new dynamic[]
                {
                    new
                    {
                        username = "John",
                        password = "Dough",
                        data = new dynamic[]
                        {
                            new
                            {
                                place = "Facebook",
                                username = "admin",
                                password = "admin"
                            }
                        }
                    },
                    new
                    {
                        username = "Jip",
                        password = "Dip",
                        data = new dynamic[]
                        {
                            new
                            {
                                place = "Facebook",
                                username = "admin",
                                password = "admin"
                            }
                        }
                    }
                }
                };

                jsonDyn = Add(jsonDyn, "John", "Dough", new LoginCredentials("Instagram", "admin", "admin"));
                jsonDyn = Add(jsonDyn, "Jip", "Dop", new LoginCredentials("Instagram", "admin", "admin"));
                Console.WriteLine(JsonConvert.SerializeObject(jsonDyn));
                List<LoginCredentials> sampleList = GetLoginsUser(jsonDyn, "John", "Dough");
                foreach (LoginCredentials cred in sampleList)
                {
                    Console.WriteLine(cred);
                }
            }


        }
        public static List<LoginCredentials> GetLoginsUser(dynamic json, string mUsername, string mPassword)
        {
            JArray userArray = JArray.Parse(JsonConvert.SerializeObject(json.data));
            bool foundUser = false;
            JArray userData = new JArray();

            foreach (var user in userArray.Children())
            {
                var userProp = user.Children<JProperty>();
                if (userProp.FirstOrDefault(x => x.Name == "username").Value.ToString() == mUsername && userProp.FirstOrDefault(x => x.Name == "password").Value.ToString() == mPassword && !foundUser)
                {
                    userData = (JArray)user.Children<JProperty>().FirstOrDefault(x => x.Name == "data").Value;
                    foundUser = true;
                }
            }
            List<LoginCredentials> returnList = new List<LoginCredentials>();
            if (foundUser)
            {
                foreach (dynamic user in userData.Children())
                {
                    returnList.Add(new LoginCredentials((string)user.place, (string)user.username, (string)user.password));
                }
            }

            return returnList;
        }


        public static dynamic Add(dynamic json, string mUsername, string mPassword, LoginCredentials login)
        {
            dynamic dynLogin = new
            {
                place = login.Place,
                username = login.Username,
                password = login.Password
            };

            JArray userArray = JArray.Parse(JsonConvert.SerializeObject(json.data));
            bool foundUser = false;

            foreach (var user in userArray.Children())
            {
                
                var userProp = user.Children<JProperty>();
                if (userProp.FirstOrDefault(x => x.Name == "username").Value.ToString() == mUsername && userProp.FirstOrDefault(x => x.Name == "password").Value.ToString() == mPassword && !foundUser)
                {
                    JArray userData = (JArray)user.Children<JProperty>().FirstOrDefault(x => x.Name == "data").Value;
                    
                    userData.Add(JObject.Parse(JsonConvert.SerializeObject(dynLogin)));
                    foundUser = true;
                }
                
            }
            if (!foundUser)
            {
                dynamic newUser = new
                {
                    username = mUsername,
                    password = mPassword,
                    data = new dynamic[]
                    {
                        dynLogin
                    }
                };
                userArray.Add(JObject.Parse(JsonConvert.SerializeObject(newUser)));
            }

            dynamic newJson = new
            {
                data = userArray
            };
            return newJson;
        }

    }
}