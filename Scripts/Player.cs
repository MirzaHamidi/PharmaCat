using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    private Vector2 velocity;
    private const float MoveSpeed = 200f;
    internal class Player : Sprite
    {
        public Player(Texture2D texture, Vector2 position) : base(texture, position) { }
    }
}