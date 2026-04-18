using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    internal class Player : Sprite
    {
        private Vector2 velocity;
        private Vector2 targetPosition;
        private const float MoveSpeed = 200f;

        public Player(Texture2D texture, Vector2 position) : base(texture, position)
        {
            targetPosition = position;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
        public void SetTargetPosition(Vector2 target)
        {
            targetPosition = target;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = targetPosition - position;

            if (direction.Length() > 5f)
            {
                direction.Normalize();
                velocity = direction * MoveSpeed;
                position += velocity * deltaTime;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }
    }
}