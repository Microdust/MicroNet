using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public class NATHost : NATRemoteBase
    {
        public string Title = "MicroNET Host";
        public string Password;
        public DateTime ListedDate;
        
        public NATHost()
        {
        
        }

        public override void Initialize(IncomingMessage msg)
        {
            base.Initialize(msg);
            HostId = CreateUniqueId(Remote.EndPoint);
            ListedDate = DateTime.UtcNow;
        }



        // Using Cantors function to create a unique identifier based on host external IP and port
        public uint CreateUniqueId(IPEndPoint point)
        {
            uint x = (uint)point.Address.Address;
            ushort y = (ushort)point.Port;

            return ((x + y) * (x + y + 1)) / 2 + y;
        }

    }
}
