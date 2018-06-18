using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MicroGodotNet.Messages;
using MicroGodotNet.Signal;
using MicroNet.Network;

namespace MicroGodotNet
{
    public class HostPlayer : NetworkPlayer
    {
        private List<PlayerBase> players = new List<PlayerBase>(4);

        public HostPlayer() : base(new NetConfiguration(5001)
        {
            Name = "HostPlayer",
            AllowConnectors = true,
            Port = 5000,
        })
        {
            Start();
        }

        public override void OnConnect(RemoteConnection remote)
        {
            PlayerBase player = new PlayerBase
            {
                Id = (int)remote.ConnectionId,               
            };

            
            
            Spawn spawn = new Spawn() { Id = (int)remote.ConnectionId, Position = new Vector2(300, 300), Color = new Color(0.85f, 0.35f, 0.15f) };
            players.Add(player);
            SignalManager.Signal(spawn);

            WorldUpdate update = new WorldUpdate();
            update.Players[0] = new Spawn() { Id = -1, Position = players[0].Position, Color = players[0].Colour };
            update.Players[1] = new Spawn() { Id = players[1].Id, Position = players[1].Position, Color = players[1].Colour };

            OutgoingMessage message = MessagePool.CreateMessage();
            update.Write(message);
           
            remote.Send(message);

            MessagePool.Recycle(message);

        }

        public override void OnConnectionFailure()
        {
            SignalManager.Signal(NetworkStatus.ConnectionFailure);
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            players.RemoveAt((int)remote.ConnectionId);
        }

        public override void OnReady()
        {
            SignalManager.Signal(NetworkStatus.Hosting);

            Spawn spawn = new Spawn() { Id = -1, Position = new Vector2(600, 600), Color = new Color(0.25f, 0.35f, 0.75f) };
            SignalManager.Signal(spawn);

            players.Add(new PlayerBase());
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch (msg.ReadInt32())
            {
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
            OutgoingMessage msg = MessagePool.CreateMessage();
            movement.Write(msg);
            Broadcast(msg);
        }
    }
}
