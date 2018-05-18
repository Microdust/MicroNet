using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe struct Address : IEquatable<Address>
    {
        public ENet.ENetAddress ENetAddress;

        public string GetHostIP()
        {
            var ip = new byte[256];
            fixed (byte* hostIP = ip)
            {
                if (ENet.AddressGetHostIp(ref ENetAddress, hostIP, (IntPtr)ip.Length) < 0)
                {
                    return null;
                }
            }
            // Doubt it
            return Encoding.ASCII.GetString(ip, 0, ip.Length);
        }

        public void SetHost(string hostName)
        {
            ENet.AddressSetHost(ref ENetAddress, Encoding.ASCII.GetBytes(hostName));
        }

        public bool Equals(Address other)
        {
            throw new NotImplementedException();
        }

    }
}
