using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    internal class LocalHost : NetworkManager
    {
        private Stopwatch watch = new Stopwatch();

        public LocalHost(NetConfiguration configuration) : base(configuration)
        {

        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": OnStop");
        }

        public override void OnReady()
        {
            Debug.Log(config.Name,": OnReady");

            if (config.AllowConnectors == true)
                return;

            Connect("127.0.0.1", 8080);
        }

        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": OnConnect");

        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log(config.Name.ToString(), ": OnDisconnect");
        }

        public override void OnReceived(IncomingMessage msg)
        {
            OutgoingMessage outgoing = MessagePool.CreateMessage();
        //    outgoing.Write(msg.ReadVector2());


            msg.Remote.Send(outgoing);

            MessagePool.Recycle(outgoing);

        }
    }
}
