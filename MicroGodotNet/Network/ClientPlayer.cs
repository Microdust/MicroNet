using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MicroGodotNet.Messages;
using MicroGodotNet.Signal;
using MicroNet.Network;

namespace MicroGodotNet.Network
{
    public class ClientPlayer : NetworkPlayer
    {
        public ClientPlayer() : base(new NetConfiguration(5001)
        {
            Name = "ClientPlayer",
            AllowConnectors = false,
            Port = 5000,
            
        })
        {
            Start();
        }

        public override void OnConnect(RemoteConnection remote)
        {
            SignalManager.Signal(NetworkStatus.ConnectionSuccess);
        }

        public override void OnConnectionFailure()
        {
            SignalManager.Signal(NetworkStatus.ConnectionFailure);
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            SignalManager.Signal(NetworkStatus.Disconnection);
        }

        public override void OnReady()
        {
            SignalManager.Signal(NetworkStatus.Ready);
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch (msg.ReadInt32())
            {
                case MessageTypes.Spawn:
                SignalManager.Signal(Spawn.Read(msg));
                break;

                case MessageTypes.SynchronizeMessage:

                WorldUpdate update = new WorldUpdate();
                update.Read(msg);

                for (int i = 0; i < 2; i++)
                    SignalManager.Signal(update.Players[i]);

                break;

                case MessageTypes.Move:
                SignalManager.Signal(Movement.Read(msg));
                break;
                case MessageTypes.WorldState:
                break;

            }
        }

        public override void OnStop()
        {
            SignalManager.Signal(NetworkStatus.Stopped);
        }

        public override void Send(Movement movement)
        {
            throw new NotImplementedException();
        }
    }
}
