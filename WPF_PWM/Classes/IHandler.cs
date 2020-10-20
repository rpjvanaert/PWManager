using General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PWM.Classes
{
    public interface IHandler
    {
        void giveData(List<LoginCredentials> logins);

        void Login(bool status);

        void stop();
    }
}
