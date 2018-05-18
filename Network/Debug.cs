﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public static class Debug
    {

        public static void Log(params string[] message)
        {
            Console.WriteLine(string.Concat(DateTime.UtcNow.ToString("dd-MM-yy HH:mm:ss"), " [Debug]: ", string.Concat(message)));
        }

        public static void Error(params string[] message)
        {
            Console.WriteLine(string.Concat(DateTime.UtcNow.ToString("dd-MM-yy HH:mm:ss"), " [Error]: ", string.Concat(message)));
        }

    }
}
