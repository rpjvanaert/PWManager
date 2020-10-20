using System;
using System.Collections.Generic;
using System.Text;

namespace PWManager
{
    class Singleton
    {
        private static Singleton intstance = new Singleton();
        private Singleton() { }
        
        public static Singleton Instance
        {
            get { return intstance; }
        }

        public void dosomething()
        {
            Console.WriteLine("yes");
        }   
    }
}
