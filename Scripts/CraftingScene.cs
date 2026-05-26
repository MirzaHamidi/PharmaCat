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

            // DÜZENLEME: Go to Shop butonu ekranın alt ortasına (güvenli bölgeye) alındı
            goToShopButton = new TextButton
            {
                Text = "Go to Shop",
                Width = 250,
                Height = 70,
                Left = 835, // 1920 piksel genişliğin tam ortası
                Top = 920   // Ekranın alt kısmı
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
            // DÜZENLEME: Masa grafiği biraz daha aşağıya (Y: 250) kaydırıldı ki havan tam üstüne otursun
            spriteBatch.Draw(tableTexture, new Vector2(0, 250), Color.White);
            craftingDesktop?.Render();
        }

        public void ResetRequest()
        {
            GoToShopRequested = false;
        }
    }
}