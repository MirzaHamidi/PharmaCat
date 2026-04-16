using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PharmaCat.Scripts
{
    internal class Sprite
    {
        public float Scale = 0.3f;

        public Texture2D texture;
        public Vector2 position;

        public float Rotation = 0f; 
        public Vector2 Origin => new Vector2(texture.Width / 2f, texture.Height / 2f);
        public float Alpha = 1f;

        public Rectangle Rect // Collision rectangle
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)(texture.Width * Scale),
                    (int)(texture.Height * Scale)
                );
            }
        }

        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
            Vector2 centerPos = position + Origin * Scale; // Center position for rotation

            spriteBatch.Draw(
                texture,
                centerPos,
                null,
                color: Color.White * Alpha,
                Rotation,
                Origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}