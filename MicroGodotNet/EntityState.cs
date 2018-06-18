using Godot;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public struct EntityState
    {
        public int Id;
        public Vector2 Position;
        public int LastProcessedInput;


        public void Write(OutgoingMessage msgOut)
        {
            msgOut.Write(Id);
            msgOut.Write(Position);
            msgOut.Write(LastProcessedInput);
        }

        public static EntityState Read(IncomingMessage msgIn)
        {
            return new EntityState()
            {
                Id = msgIn.ReadInt32(),
                Position = msgIn.ReadVector2(),
                LastProcessedInput = msgIn.ReadInt32()
            };
        }
    }
}
