using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    internal class TiledIsoEntity
    {
        public Texture2D Texture;
        public Rectangle SourceRect;
        public Vector2 Position;
        public bool IsBush;
        public bool Collected;
        public float Alpha = 1f;

        public float SortY => Position.Y;

        public TiledIsoEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, bool isBush)
        {
            Texture = texture;
            SourceRect = sourceRect;
            Position = position;
            IsBush = isBush;
        }

        public void Draw(SpriteBatch spriteBatch)
{
    if (Collected) return;

    Vector2 origin = new Vector2(
        SourceRect.Width / 2f,
        SourceRect.Height
    );

    spriteBatch.Draw(
        Texture,
        Position,
        SourceRect,
        Color.White * Alpha,
        0f,
        origin,
        1f,
        SpriteEffects.None,
        0f
    );
}
    }
}