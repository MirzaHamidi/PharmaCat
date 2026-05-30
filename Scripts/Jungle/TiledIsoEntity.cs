using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts.World
{
    internal class TiledIsoEntity
    {
        private Texture2D _texture;
        private Rectangle _sourceRect;
        private bool _isBush;
        public bool Collectable = false;
        public float Scale = 1f;
        public float TargetScale = 1f;

        public bool IsCollecting = false;
        public bool ShouldRemove = false;

        private float collectTimer = 0f;
        private const float collectDuration = 0.5f;

        public Vector2 Position { get; set; }
        public float Alpha { get; set; } = 1f;
        public bool Collected { get; set; }

        public bool IsBush => _isBush;
        public int LocalId { get; private set; }
        public Rectangle SourceRect => _sourceRect;

        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - (_sourceRect.Width * Scale) / 2f),
            (int)(Position.Y - (_sourceRect.Height * Scale)),
            (int)(_sourceRect.Width * Scale),
            (int)(_sourceRect.Height * Scale)
        );

        public TiledIsoEntity(Texture2D texture, Rectangle sourceRect, Vector2 position, bool isBush, int localId = -1)
        {
            _texture = texture;
            _sourceRect = sourceRect;
            Position = position;
            _isBush = isBush;
            LocalId = localId;
        }

        public void UpdateVisual(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsCollecting)
            {
                collectTimer += dt;
                float t = collectTimer / collectDuration;

                Scale = MathHelper.Lerp(1f, 0f, t);

                if (t >= 1f)
                {
                    Scale = 0f;
                    ShouldRemove = true;
                }

                return;
            }

            Scale = MathHelper.Lerp(
                Scale,
                TargetScale,
                10f * dt
            );
        }

        public void StartCollectAnimation()
        {
            IsCollecting = true;
            collectTimer = 0f;
            TargetScale = 0f;
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
            if (ShouldRemove)
                return;

            Vector2 origin = new Vector2(
                _sourceRect.Width / 2f,
                _sourceRect.Height
            );

            spriteBatch.Draw(
                _texture,
                Position,
                _sourceRect,
                Color.White * Alpha,
                0f,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}