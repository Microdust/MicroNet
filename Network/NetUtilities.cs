using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{

    public static class NetUtilities
    {
        public static byte[] StringBuffer = new byte[255];

        public static NetSingle WriteSingle = new NetSingle();
        public static NetSingle ReadSingle = new NetSingle();

    }
}
