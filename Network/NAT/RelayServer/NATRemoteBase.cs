using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public abstract class NATRemoteBase
    {
        public ulong HostId = 0; // Unique ID for the host, clients use this value to announce which host they join
       // public IPEndPoint Internal;
        internal RemoteConnection Remote;

        public IPEndPoint External
        {
            get { return Remote.EndPoint; }
        }


        public virtual void Initialize(IncomingMessage msg)
        {
            Remote = msg.Remote;
//            External = new IPEndPoint(Remote.IPAddress, Remote.EndPoint.Port);
        }

        public void Introduce(NATRemoteBase conn)
        {
            Debug.Log("NATServer: Introducing: ", conn.Remote.EndPoint.ToString(), " to: ", Remote.IPAddress.ToString());
            OutgoingMessage message = MessagePool.CreateMessage();

            message.Write(NATMessageType.INTRODUCTION);
            message.Write(conn.Remote.EndPoint);

            Remote.Send(message);

            MessagePool.Recycle(message);
        }

    }
}
