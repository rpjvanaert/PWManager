﻿using General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server
{
    class SaveData
    {
        private string path;
        private dynamic json;
        private const string jsonFilename = "/json.txt";
        public SaveData(string path)
        {
            this.path = path;
            this.ReadJSON();
        }

        public void ReadJSON()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                json = new { };
            }
            else
            {
                string file = "";
                using (StreamReader sr = new StreamReader(this.path + jsonFilename))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        file += line;
                    }
                    json = JsonConvert.DeserializeObject(file);
                }
            }
        }

        public void WriteDataJSON(string data)
        {
            using (StreamWriter sw = File.AppendText(this.path + jsonFilename))
            {
                sw.WriteLine(data);
            }
        }

        public List<LoginCredentials> GetLoginsUser(string mUsername, string mPassword)
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

        public void Add(string mUsername, string mPassword, LoginCredentials login)
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
            json = newJson;
        }
    }
}

