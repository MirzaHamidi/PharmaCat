using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace PharmaCat.Scripts
{
    internal class CraftingScene
    {
        private Desktop craftingDesktop;
        private Window sellWindow;
        private TextBox priceInput;
        private PotionGlassBox currentSellingGlass;

        public Action<PotionGlassBox, int> OnSellConfirmed;

        public void Load() 
        {
            var panel = new Panel();

            sellWindow = new Window
            {
                Title = "Offer Price",
                Left = 1920 / 2 - 150,
                Top = 1080 / 2 - 100,
                Width = 300,
                Height = 200,
                Visible = false
            };

            var verticalStack = new VerticalStackPanel { Spacing = 15, Padding = new Myra.Graphics2D.Thickness(20) };
            var lbl = new Label { Text = "Set your price ($):" };
            priceInput = new TextBox { Text = "20" };
            
            var sellBtn = new TextButton { Text = "Offer to Customer", Width = 200, Height = 40 };
            sellBtn.Click += (s, a) =>
            {
                if (int.TryParse(priceInput.Text, out int price))
                {
                    OnSellConfirmed?.Invoke(currentSellingGlass, price);
                    sellWindow.Visible = false;
                }
            };

            var cancelBtn = new TextButton { Text = "Cancel", Width = 200, Height = 40 };
            cancelBtn.Click += (s, a) =>
            {
                sellWindow.Visible = false;
            };

            verticalStack.Widgets.Add(lbl);
            verticalStack.Widgets.Add(priceInput);
            verticalStack.Widgets.Add(sellBtn);
            verticalStack.Widgets.Add(cancelBtn);
            sellWindow.Content = verticalStack;

            panel.Widgets.Add(sellWindow);

            craftingDesktop = new Desktop();
            craftingDesktop.Root = panel;
        }

        public void OpenSellUI(PotionGlassBox glass)
        {
            currentSellingGlass = glass;
            priceInput.Text = "20"; 
            sellWindow.Visible = true;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            craftingDesktop?.Render();
        }
    }
}