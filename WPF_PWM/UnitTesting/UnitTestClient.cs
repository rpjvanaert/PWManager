using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace WPF_PWM.UnitTesting
{
    [TestClass]
    public class UnitTestClient
    {

        [TestMethod]
        public void TestLoginMessage()
        {
            dynamic dataCorrect = new
            {
                id = "DATA_RESPONSE",
                data = new
                {
                    status = "OK",
                    data = new dynamic[]
                    {
                        new 
                        {
                            place = "admin",
                            username = "admina",
                            password = "adminb"
                        }
                    }
                }
            };
            List<LoginCredentials> correctList = new List<LoginCredentials>();
            correctList.Add(new LoginCredentials("admin", "admina", "adminb"));
            byte[] dataParser = DataParser.GetDataResponse("OK", correctList);

            LoginCredentials correctLogin = new LoginCredentials("admin", "admina", "adminb");

            
            LoginCredentials dynAnswer = DataParser.GetData(dataCorrect).get(0);

            Assert.Equals(dynAnswer.Place, correctLogin.Place);
            Assert.Equals(dynAnswer.Username, correctLogin.Username);
            Assert.Equals(dynAnswer.Password, correctLogin.Password);


            LoginCredentials parserAnswer = DataParser.GetData((dynamic)dataParser).get(0);

            Assert.Equals(dynAnswer.Place, correctLogin.Place);
            Assert.Equals(dynAnswer.Username, correctLogin.Username);
            Assert.Equals(dynAnswer.Password, correctLogin.Password);
        }
    }
}
