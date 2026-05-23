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

        public bool IsBush => _isBush;
        public int LocalId { get; private set; }
        public Rectangle SourceRect => _sourceRect;

        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - _sourceRect.Width / 2f),
            (int)(Position.Y - _sourceRect.Height),
            _sourceRect.Width,
            _sourceRect.Height
        );

        public TiledIsoEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, bool isBush, int localId = -1)
        {
            _texture = texture;
            _sourceRect = sourceRect;
            Position = position;
            _isBush = isBush;
            LocalId = localId;
        }

        public bool ContainsPoint(Vector2 worldPoint)
        {
            return Bounds.Contains((int)worldPoint.X, (int)worldPoint.Y);
        }

        public string GetPlantName()
        {
            if (!_isBush)
                return "Tree";

            return LocalId switch
            {
                22 => "Lavender",
                23 => "Blue Lotus",
                24 => "Love Rose",
                25 => "Anti-Curse Clover",
                36 => "Sage",
                37 => "Red Poppy",
                38 => "Marigold",
                _ => $"Wild Herb ({LocalId})"
            };
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
