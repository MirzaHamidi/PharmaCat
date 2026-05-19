using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts.World
{
    internal class TiledIsoEntity
    {
        private Texture2D _texture;
        private Rectangle _sourceRect;
        private bool _isBush;

        public Vector2 Position { get; set; }
        public float Alpha { get; set; } = 1f;
        public bool Collected { get; set; }

        public TiledIsoEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, bool isBush)
        {
            _texture = texture;
            _sourceRect = sourceRect;
            Position = position;
            _isBush = isBush;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Collected)
                return;

            Vector2 origin = new Vector2(_sourceRect.Width / 2f, _sourceRect.Height);

            spriteBatch.Draw(
                _texture,
                Position,
                _sourceRect,
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
