using Godot;
using MicroGodotNet.Messages;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public abstract class Entity : Sprite
    {
        public int Id;
        public abstract void ApplyInput(Movement move);
        public Color Colour;


        public void LoadTexture()
        {
            Texture = ResourceLoader.Load("res://icon.png") as Texture;
        }

    }
}
