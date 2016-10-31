using System;
using System.Collections.Generic;
using Swift;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server srv = new Server(args);

            srv.Start();
        }
    }
}
