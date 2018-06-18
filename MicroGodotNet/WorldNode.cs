using Godot;
using MicroGodotNet.Messages;
using MicroGodotNet.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public class WorldNode : Node, ISignal<Spawn> , ISignal<Movement> , ISignal<NetworkPlayer>
    {
        private List<PlayerBase> players = new List<PlayerBase>(4);
        public NetworkPlayer network;

        public override void _Ready()
        {
            SignalManager.Subscribe<Spawn>(this);
            SignalManager.Subscribe<NetworkPlayer>(this);

        
        }


        public void Execute(NetworkPlayer message)
        {
            network = message;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (network == null)
                return;

            network.Tick();

        }

        public void Execute(Spawn message)
        {
            PlayerBase player = new PlayerBase();
            player.Spawn(message);
            players.Add(player);

            AddChild(player);
        }


        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        public void Execute(Movement message)
        {
            players[message.Id].Position = message.Input;
        }
    }
}
