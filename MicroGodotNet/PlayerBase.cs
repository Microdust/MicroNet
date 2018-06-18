using Godot;
using MicroGodotNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public class PlayerBase : Entity
    {
        public void Spawn(Spawn info)
        {
            this.LoadTexture();
            Id = info.Id;
            Position = info.Position;
            this.SelfModulate = info.Color;

            GD.Print(info.Id, ", ", "Position: ", info.Position.x, ", ", info.Position.y);
        }


        public override void ApplyInput(Movement data)
        {
            Vector2 input = data.Input;
            if (input.LengthSquared() == 0f)
            {
                return;
            }

            this.Position = input * GameConstants.PlayerSpeed;
        }
    }
}
