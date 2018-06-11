using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{

    internal static class NetUtilities
    {
        public static byte[] EmptyByteArray = new byte[255];

        public static NetSingle WriteSingle = new NetSingle();
        public static NetSingle ReadSingle = new NetSingle();


        // Using Cantors function to create a unique identifier based on host external IP and port
        public static uint CreateUniqueId(IPEndPoint point)
        {
            uint x = (uint)point.Address.Address;
            ushort y = (ushort)point.Port;

            return ((x + y) * (x + y + 1)) / 2 + y;
        }


        public static IPAddress GetLocalAddress()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface ni = null;

            for (int i = 0; i < interfaces.Length; i++)
            {
                ni = interfaces[i];
                if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    break;
            }

            if (ni == null)
                return null;
            

            IPInterfaceProperties properties = ni.GetIPProperties();

            foreach (UnicastIPAddressInformation unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress != null && unicastAddress.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return unicastAddress.Address;
                }
            }

            return null;
        }



    }
}
