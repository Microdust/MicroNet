using Godot;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Messages
{
    public class WorldUpdate
    {
        public readonly Spawn[] Players = new Spawn[4];

        public void Write(OutgoingMessage msgOut)
        {
            msgOut.Write(MessageTypes.SynchronizeMessage);

            msgOut.Write(Players.Length);

            for (int i = 0; i < Players.Length; i++)
            {
                msgOut.Write(Players[i].Id);
                msgOut.Write(Players[i].Position);
                msgOut.Write(Players[i].Color);
            }

        }

        public void Read(IncomingMessage msgIn)
        {
            int count = msgIn.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                Players[i] = Spawn.Read(msgIn);
            }
        }
    }
}
