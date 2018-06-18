using Godot;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Messages
{
    public struct Movement
    {
        public int Id;
        public int Sequence;
        public Vector2 Input;

        public void Write(OutgoingMessage msgOut)
        {
            msgOut.Write((byte)MessageTypes.Move);
            msgOut.Write(Id);
            msgOut.Write(Input);
            msgOut.Write(Sequence);
        }

        public static Movement Read(IncomingMessage msgIn)
        {
            return new Movement()
            {
                Id = msgIn.ReadInt32(),
                Input = msgIn.ReadVector2(),
                Sequence = msgIn.ReadInt32(),
            };
        }
    }
}
