using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{ 
    public class NATRemote
    {
        public RemoteConnection Remote;


        public NATRemote(RemoteConnection remote)
        {
            this.Remote = remote;
        }

        // Param == connection to be introduced
        public void Introduce(NATRemote conn)
        {
            Debug.Log("NATServer: Introducing: ", conn.Remote.EndPoint.ToString(), " to: ", Remote.EndPoint.ToString());
            OutgoingMessage message = MessagePool.CreateMessage();

            message.Write(NATMessageType.INTRODUCTION);
            message.Write(conn.Remote.EndPoint);

            Remote.Send(message);

            MessagePool.Recycle(message);
        }

    }
}
