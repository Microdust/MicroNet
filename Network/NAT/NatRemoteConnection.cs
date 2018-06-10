using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public class NatRemoteConnection
    {
        public IPEndPoint InternalIp;
        public IPEndPoint ExternalIp;

        private RemoteConnection remote;

     //   public bool isHost = false;

        public string Password;
        public ulong HostingId;


        public void Initialize(IncomingMessage msg)
        {
            remote = msg.Remote;

            InternalIp = msg.ReadIPEndPoint();      
            ExternalIp = new IPEndPoint(remote.IPAddress, InternalIp.Port);
            HostingId = msg.ReadUInt32();
            Password = msg.ReadString();
        }

        public void Introduce(NatRemoteConnection conn)
        {
            Debug.Log("NATServer: Introducing: ", conn.ExternalIp.ToString(), " to: ", remote.IPAddress.ToString());
            OutgoingMessage message = MessagePool.CreateMessage();

            message.Write(NATMessageType.INTRODUCTION);

            message.Write(conn.InternalIp);
            message.Write(conn.ExternalIp);
            message.WriteString(conn.Password);

            remote.Send(message);

            MessagePool.Recycle(message);
        }

    }
}
