﻿using General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PWM.Classes
{
    public interface IDataWindow
    {
        void Message(string message);

        void GiveData(List<LoginCredentials> logins);

        void Stop();
    }

    public interface ILoginWindow
    {
        void Login(bool status);

        void Message(string message); 
    }
}
