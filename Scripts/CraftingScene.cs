using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class CraftingScene
    {
        private Desktop craftingDesktop;
        private TextButton goToShopButton;
        private Texture2D tableTexture;

        public bool GoToShopRequested { get; private set; }

        public void Load(Texture2D tableTexture)
        {
            this.tableTexture = tableTexture;

            var panel = new Panel();

            goToShopButton = new TextButton
            {
                Text = "Go to Shop",
                Width = 220,
                Height = 70,
                Left = 850,
                Top = 500
            };

            goToShopButton.Click += (s, a) =>
            {
                GoToShopRequested = true;
            };

            panel.Widgets.Add(goToShopButton);

            craftingDesktop = new Desktop();
            craftingDesktop.Root = panel;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tableTexture, Vector2.Zero, Color.White);
            craftingDesktop?.Render();
        }

        public void ResetRequest()
        {
            GoToShopRequested = false;
        }
    }
}