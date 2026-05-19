using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class MainMenuScene
    {
        private Desktop menuDesktop;
        private TextButton startButton;

        public bool StartRequested { get; private set; }

        public void Load()
        {
            var panel = new Panel();

            startButton = new TextButton
            {
                Text = "Start Game",
                Width = 220,
                Height = 70,
                Left = 850,
                Top = 500
            };

            startButton.Click += (s, a) =>
            {
                StartRequested = true;
            };

            panel.Widgets.Add(startButton);

            menuDesktop = new Desktop();
            menuDesktop.Root = panel;
        }

        public void Update(GameTime gameTime)
        {
            // Şimdilik boş kalabilir
        }

        public void Draw()
        {
            menuDesktop?.Render();
        }

        public void ResetRequest()
        {
            StartRequested = false;
        }
    }
}