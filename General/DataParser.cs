using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace General
{
    public class DataParser
    {
        public const string LOGIN = "LOGIN";
        public const string LOGIN_RESPONSE = "LOGIN_RESPONSE";
        public const string DATA = "DATA";
        public const string DATA_RESPONSE = "DATA_RESPONSE";
        public const string ADD = "ADD";
        public const string ADD_RESPONSE = "ADD_RESPONSE";
        public const string DELETE = "DELETE";
        public const string DELETE_RESPONSE = "DELETE_RESPONSE";

        public static bool getJsonIdentifier(byte [] message, out string identifier)
        {
            if (message.Length <= 5)
            {
                throw new ArgumentException("Message too short.");
            }
            byte messageId = message[4];

            if (messageId == 0x01)
            {
                dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(message.Skip(5).ToArray()));
                identifier = json.id;
                return true;
            } 
            else
            {
                identifier = "";
                return false;
            }
        }
        public static bool GetUsernamePassword(byte[] jsonbytes, out string username, out string password)
        {
            dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(jsonbytes));
            try
            {
                username = json.data.username;
                password = json.data.password;
                return true;
            }
            catch
            {
                username = null;
                password = null;
                return false;
            }
        }

        private static byte[] GetMessage(byte[] payload, byte messageId)
        {
            byte[] res = new byte[payload.Length + 5];
            Array.Copy(BitConverter.GetBytes(payload.Length + 5), 0, res, 0, 4);
            res[4] = messageId;
            Array.Copy(payload, 0, res, 5, payload.Length);

            return res;
        }

        public static byte[] GetJsonMessage(byte[] payload)
        {
            return GetMessage(payload, 0x01);
        }

        public static byte[] GetJsonMessage(string message)
        {
            return GetJsonMessage(Encoding.ASCII.GetBytes(message));
        }

        private static byte[] GetJsonMessage(string id, dynamic data)
        {
            dynamic json = new
            {
                id = id,
                data
            };
            return GetMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json)), 0x01);
        }

        public static byte[] GetLoginResponse(string mStatus)
        {
            return GetJsonMessage(LOGIN_RESPONSE, new { status = mStatus });
        }

        public static byte[] GetDataResponse(string mStatus)
        {
            return GetJsonMessage(DATA_RESPONSE, new { status = mStatus });
        }

        public static byte[] GetAddResponse(string mStatus)
        {
            return GetJsonMessage(ADD_RESPONSE, new { status = mStatus });
        }

        public static byte[] GetDataResponse(string mStatus, List<LoginCredentials> logins)
        {
            dynamic data = new
            {
                status = mStatus,
                data = logins.ToArray()
            };

            return GetJsonMessage(DATA_RESPONSE, data);
        }

        public static byte[] GetDeleteResponse(string mStatus)
        {
            return GetJsonMessage(DELETE_RESPONSE, new { status = mStatus });
        }

        public static List<LoginCredentials> GetData(byte[] payload)
        {
            dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(payload));

            JArray userArray = JArray.Parse(JsonConvert.SerializeObject(json.data.data));
            List<LoginCredentials> logins = new List<LoginCredentials>();

            foreach (var user in userArray.Children())
            {
                JArray userData = (JArray)user.Children<JProperty>().FirstOrDefault(x => x.Name == "data").Value;
                foreach (dynamic login in userData)
                {
                    logins.Add(new LoginCredentials((string)login.place, (string)login.username, (string)login.password));
                }
            }

            return logins;
        }

        public static bool GetAddition(byte[] jsonbytes, out LoginCredentials login)
        {
            dynamic json = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(jsonbytes));

            try
            {
                login = new LoginCredentials(json.data.data.place, json.data.data.username, json.data.data.password);
                return true;
            }
            catch
            {
                login = new LoginCredentials(null, null, null);
                return false;
            }
        }


    }
}
