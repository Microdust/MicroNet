using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public class NATHostInfo
    {
        public string Title = "MicroNET Host";
    //    public DateTime ListedDate;
        public ulong HostId = 0;
    //    public IPEndPoint EndPoint;



        public void ReadMessage(IncomingMessage msg)
        {
       //     Title = msg.ReadString();
            HostId = msg.ReadUInt64();          
        }

        public void WriteMessage(OutgoingMessage msg)
        {
   //         msg.WriteString(Title);
            msg.Write(HostId);
        }

    }
}
