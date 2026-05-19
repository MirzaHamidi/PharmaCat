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

        public float SortY => Position.Y;

        public Rectangle InteractionRect => new Rectangle(
            (int)Position.X - 35,
            (int)Position.Y - 30,
            70,
            60
        );

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

    Vector2 origin;

    if (SourceRect.Width == Texture.Width && SourceRect.Height == Texture.Height)
        origin = Vector2.Zero; // full layer PNG
    else
        origin = new Vector2(SourceRect.Width / 2f, SourceRect.Height); // tek obje

    spriteBatch.Draw(
        Texture,
        Position,
        SourceRect,
        Color.White,
        0f,
        origin,
        1f,
        SpriteEffects.None,
        0f
    );
}
    }
}