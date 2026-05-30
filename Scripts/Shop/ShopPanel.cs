using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    public partial class CraftGreyboxSystem
    {
        private void HandleShopClicks(Point mp)
        {
            if (!LeftPressed())
            {
                return;
            }

            string[] herbNames = new string[] { "Lavender", "Blue Lotus", "Love Rose", "Anti-Curse Clover", "Sage", "Red Poppy", "Marigold" };

            for (int i = 0; i < herbNames.Length; i++)
            {
                Rectangle herbButton = new Rectangle(shopPanel.X + 40 + (i % 4) * 230, shopPanel.Y + 90 + (i / 4) * 80, 210, 55);
                
                if (herbButton.Contains(mp) && inventory.SpendMoney(15))
                {
                    inventory.AddHerb(herbNames[i], 1);
                    CreateJarsFromInventory();
                }
            }

            Rectangle buyBottle = new Rectangle(shopPanel.X + 40, shopPanel.Y + 280, 210, 55);
            if (buyBottle.Contains(mp) && inventory.SpendMoney(25))
            {
                inventory.EmptyBottleCount++;
                CreateGlassesFromInventory();
            }

            Rectangle buyMortar = new Rectangle(shopPanel.X + 270, shopPanel.Y + 280, 250, 55);
            if (buyMortar.Contains(mp) && inventory.SpendMoney(80))
            {
                inventory.MortarLevel++;
            }
        }

        private void DrawShop(SpriteBatch sb)
        {
            DrawBox(sb, shopPanel, new Color(25, 25, 25, 250));
            sb.DrawString(font, "MERCHANT SHOP", new Vector2(shopPanel.X + 40, shopPanel.Y + 30), Color.Gold);

            DrawBox(sb, closeShopButton, Color.DarkRed);
            sb.DrawString(font, "X", new Vector2(closeShopButton.X + 11, closeShopButton.Y + 8), Color.White);

            string[] herbNames = new string[] { "Lavender", "Blue Lotus", "Love Rose", "Anti-Curse Clover", "Sage", "Red Poppy", "Marigold" };

            for (int i = 0; i < herbNames.Length; i++)
            {
                Rectangle herbButton = new Rectangle(shopPanel.X + 40 + (i % 4) * 230, shopPanel.Y + 90 + (i / 4) * 80, 210, 55);
                DrawShopButton(sb, herbButton, herbNames[i] + " $15");
            }

            Rectangle buyBottle = new Rectangle(shopPanel.X + 40, shopPanel.Y + 280, 210, 55);
            DrawShopButton(sb, buyBottle, "Bottle $25");

            Rectangle buyMortar = new Rectangle(shopPanel.X + 270, shopPanel.Y + 280, 250, 55);
            DrawShopButton(sb, buyMortar, "Upgrade Mortar $80");
        }

        private void DrawShopButton(SpriteBatch sb, Rectangle r, string text)
        {
            DrawBox(sb, r, Color.DarkOliveGreen);
            sb.DrawString(font, text, new Vector2(r.X + 15, r.Y + 16), Color.White);
        }
    }
}
