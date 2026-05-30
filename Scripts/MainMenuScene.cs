using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    internal class MainMenuScene
    {
        private Texture2D texBackground;
        private Texture2D texBook;
        private Texture2D texPlayButton;

        private MouseState mouseState;
        private MouseState oldMouseState;

        private Rectangle playButtonRect;
        private bool isHovering;

        public bool StartRequested { get; private set; }

        public void Load(ContentManager content)
        {
            texBackground = content.Load<Texture2D>("1");
            texBook = content.Load<Texture2D>("2");
            texPlayButton = content.Load<Texture2D>("3");
        }

        public void Update(GameTime gameTime)
        {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            // --- YENİ: BOYUT VE KONUM AYARLARI ---
            
            // "Play" butonunun beğendiğin boyutu (%40)
            float playBaseScale = 0.40f; 
            
            // DÜZENLEME: Görünmez boşluklardan dolayı çok sola kayan yazıyı SAĞA ÇEKTİK!
            // (Eğer hala milimetrik olarak sağa/sola gitmesi gerekirse 760f sayısını değiştirebilirsin)
            float playX = 760f; 
            float playY = 600f; 

            Vector2 playPosition = new Vector2(playX, playY);

            // Tıklama alanı hesaplaması
            int playWidth = (int)(texPlayButton.Width * playBaseScale);
            int playHeight = (int)(texPlayButton.Height * playBaseScale);

            playButtonRect = new Rectangle(
                (int)(playPosition.X - playWidth / 2f),
                (int)(playPosition.Y - playHeight / 2f),
                playWidth,
                playHeight
            );

            // Fare butonun üzerinde mi?
            isHovering = playButtonRect.Contains(mouseState.Position);

            // Tıklama kontrolü
            if (isHovering && mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                StartRequested = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // 1. Arka Plan Çizimi
            spriteBatch.Draw(texBackground, new Rectangle(0, 0, 1920, 1080), Color.White);

            // 2. Kitap Çizimi (%85 ölçeğinde)
            float bookScale = 0.85f; 
            Vector2 bookOrigin = new Vector2(texBook.Width / 2f, texBook.Height / 2f); 
            Vector2 bookPosition = new Vector2(1920 / 2f, 1080 / 2f); 
            
            spriteBatch.Draw(texBook, bookPosition, null, Color.White, 0f, bookOrigin, bookScale, SpriteEffects.None, 0f);

            // 3. Play Butonu Çizimi (KAYDIRILMIŞ HALİYLE)
            float playBaseScale = 0.40f; 
            
            // Update'deki ile aynı koordinatlar (SAĞA ÇEKİLDİ)
            float playX = 730f; 
            float playY = 400f; 
            Vector2 playPosition = new Vector2(playX, playY);
            
            Vector2 playOrigin = new Vector2(texPlayButton.Width / 2f, texPlayButton.Height / 2f); 
            
            // Hover efekti: Fare üstündeyse %40'tan %46'ya çıkar (%15 büyüme), rengi parlar
            float currentScale = isHovering ? playBaseScale * 1.15f : playBaseScale;
            Color color = isHovering ? Color.White : new Color(220, 220, 220); 
            
            spriteBatch.Draw(texPlayButton, playPosition, null, color, 0f, playOrigin, currentScale, SpriteEffects.None, 0f);
        }

        public void ResetRequest()
        {
            StartRequested = false;
        }
    }
}