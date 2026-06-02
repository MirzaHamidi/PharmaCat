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

        Texture2D pixelTexture;


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

            
            float playBaseScale = 0.40f; 
            
            
            float playX = 730f; 
            float playY = 400f; 

            Vector2 playPosition = new Vector2(playX, playY);

            
            int playWidth = (int)(texPlayButton.Width * playBaseScale *0.85f);
            int playHeight = (int)(texPlayButton.Height * playBaseScale* 0.85f);

            playButtonRect = new Rectangle(
                (int)(playPosition.X - playWidth / 2f),
                (int)(playPosition.Y - playHeight / 2f),
                playWidth,
                playHeight
            );

            
            isHovering = playButtonRect.Contains(mouseState.Position);

            
            if (isHovering && mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                StartRequested = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(texBackground, new Rectangle(0, 0, 1920, 1080), Color.White);

            
            float bookScale = 0.85f; 
            Vector2 bookOrigin = new Vector2(texBook.Width / 2f, texBook.Height / 2f); 
            Vector2 bookPosition = new Vector2(1920 / 2f, 1080 / 2f); 
            
            spriteBatch.Draw(texBook, bookPosition, null, Color.White, 0f, bookOrigin, bookScale, SpriteEffects.None, 0f);

            
            float playBaseScale = 0.40f; 
            
            
            float playX = 730f; 
            float playY = 400f; 
            Vector2 playPosition = new Vector2(playX, playY);
            
            Vector2 playOrigin = new Vector2(texPlayButton.Width / 2f, texPlayButton.Height / 2f); 
            
            
            float currentScale = isHovering ? playBaseScale * 1.15f : playBaseScale;
            Color color = isHovering ? Color.White : Color.Red; 
            
            spriteBatch.Draw(texPlayButton, playPosition, null, color, 0f, playOrigin, currentScale, SpriteEffects.None, 0f);
        }

        public void ResetRequest()
        {
            StartRequested = false;
        }
    }
}