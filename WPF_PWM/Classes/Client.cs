using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PWM.Classes
{
    public class Client
    {
        private Client()
        {

        }

        private static readonly Client client = new Client();

        public static Client GetInstance()
        {
            return client;
        }
    }
}
