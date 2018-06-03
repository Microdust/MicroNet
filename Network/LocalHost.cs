using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public class LocalHost : NetworkManager
    {
        private OutgoingMessage outgoing = new OutgoingMessage(DeliveryMethod.None, 32);
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
            outgoing.WriteBool(true);




            remote.Send(outgoing);
            
        }

        public override void OnDisconnect(RemoteConnection remote)
        {

            if (config.AllowConnectors == false)
            {
                Connect("127.0.0.1", 8080);
            }

        }

        public override void OnReceived(IncomingMessage msg)
        {
            //   Debug.Log(config.Name, ": OnReceived");


            if (config.AllowConnectors == false)
            {
                Debug.Log(msg.ReadBool().ToString());
                
            }

            msg.Remote.Send(outgoing);

        }
    }
}
