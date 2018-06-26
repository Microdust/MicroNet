using Godot;
using MicroGodotNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public static class InputManager
    {
        private static List<Movement> pendingMovement = new List<Movement>();

        private static int movementSequence = 0;



        public static Vector2 GetInput()
        {
            Vector2 move = new Vector2();

            if (Input.IsKeyPressed((int)KeyList.W))
            {
                move.y = -1f;
            }
            else if (Input.IsKeyPressed((int)KeyList.S))
            {
                move.y = 1f;
            }
            if (Input.IsKeyPressed((int)KeyList.A))
            {
                move.x = -1f;
            }
            else if (Input.IsKeyPressed((int)KeyList.D))
            {
                move.x = 1f;
            }

            if (move.LengthSquared() > 1f)
            {
                return move.Normalized();
            }

            return move;

        }

    }
}
