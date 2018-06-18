using Godot;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Messages
{
    public struct Spawn
    {
        public int Id;
        public Vector2 Position;
        public Color Color;
        

        public void Write(OutgoingMessage msgOut)
        {
            msgOut.Write(MessageTypes.Spawn);
            msgOut.Write(Id);
            msgOut.Write(Position);
            msgOut.Write(Color);
        }

        public static Spawn Read(IncomingMessage msgIn)
        {
            return new Spawn()
            {
                Id = msgIn.ReadInt32(),
                Position = msgIn.ReadVector2(),
                Color = msgIn.ReadColor()
            };
        }
    }
}
