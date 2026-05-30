using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    public partial class CraftGreyboxSystem
    {
        private void DrawMoney(SpriteBatch sb)
        {
            sb.DrawString(font, $"Money: ${inventory.Money}", new Vector2(30, 30), Color.Gold);
            sb.DrawString(font, $"Mortar Level: {inventory.MortarLevel}", new Vector2(30, 55), Color.White);
            sb.DrawString(font, $"Empty Bottles: {inventory.EmptyBottleCount}", new Vector2(30, 80), Color.White);
        }

        private void DrawJars(SpriteBatch sb)
        {
            foreach (var jar in jars)
            {
                Rectangle r = jar.Bounds;
                r.X -= (int)slideOffset; 
                
                if (draggedJar == jar)
                {
                    r = new Rectangle((int)jar.DragPosition.X, (int)jar.DragPosition.Y, r.Width, r.Height);
                }

                sb.Draw(GetJarTexture(jar.Name), r, Color.White);

                sb.DrawString(font, jar.Name, new Vector2(r.X, r.Y - 20), Color.White);
                sb.DrawString(font, "x" + jar.Amount, new Vector2(r.X + 32, r.Bottom - 24), Color.White);
            }
        }

        private void DrawMortar(SpriteBatch sb)
        {
            Rectangle mBounds = mortar.Bounds;
            mBounds.X -= (int)slideOffset; 
           
            sb.Draw(texMortar, mBounds, Color.White);

            if (mortar.HasBottom)
            {
                sb.Draw(texMortarDust, mBounds, mortar.BottomColor);
            }

            if (mortar.HasTop)
            {
                sb.Draw(texMortarDust1, mBounds, mortar.TopColor);
            }

            if (hasMixedPotion)
            {
                Color potionDustColor = mixedColor * 0.9f;

                sb.Draw(texMortarDust, mBounds, potionDustColor);
                sb.Draw(texMortarDust1, mBounds, potionDustColor);
            }

            if (hasWater && !hasMixedPotion)
            {
                sb.Draw(texMortarDust, mBounds, new Color(80, 170, 255) * 0.25f);
                sb.Draw(texMortarDust1, mBounds, new Color(80, 170, 255) * 0.25f);
            }

            Rectangle gBounds = grinderCurrentRect;
            gBounds.X -= (int)slideOffset; 
            
            sb.Draw(texGrinder, gBounds, Color.White);

            sb.DrawString(font, "MORTAR", new Vector2(mBounds.X, mBounds.Y - 24), Color.White);
            sb.DrawString(font, "1) Drag herbs  2) Add water  3) Hold pestle  4) Drag mortar to glass", new Vector2(mBounds.X - 120, mBounds.Bottom + 8), Color.White);

            Rectangle barBack = new Rectangle(mBounds.X, mBounds.Bottom + 34, mBounds.Width, 12);
            Rectangle barFill = new Rectangle(barBack.X, barBack.Y, (int)(barBack.Width * (grindProgress / grindNeeded)), barBack.Height);

            sb.Draw(pixel, barBack, Color.DarkRed);
            sb.Draw(pixel, barFill, Color.LimeGreen);

            if (!string.IsNullOrEmpty(craftedPotionName) && hasMixedPotion)
            {
                sb.DrawString(font, craftedPotionName, new Vector2(mBounds.X, mBounds.Bottom + 52), Color.Gold);
            }
        
            bool mortarHasSomething = mortar.HasBottom || mortar.HasTop || hasWater || hasMixedPotion;

            float mortar1Alpha = mortarHasSomething ? 0.5f : 1f;

            sb.Draw(texMortar1, mBounds, Color.White * mortar1Alpha);
        }

        private void DrawWater(SpriteBatch sb)
        {
            Rectangle wRect = waterBox;
            wRect.X += (int)slideOffset; 
            
            DrawBox(sb, wRect, Color.CornflowerBlue);
            sb.DrawString(font, "Water", new Vector2(wRect.X + 8, wRect.Y - 22), Color.White);
        }

        private void DrawGlasses(SpriteBatch sb)
        {
            foreach (var glass in glasses)
            {
                Rectangle r = glass.Bounds;
                
                if (!glass.IsFilled) 
                {
                    r.X += (int)slideOffset;
                }
                
                if (draggedGlass == glass) 
                {
                    continue;
                }

                if (glass.IsFilled)
                {
                    sb.Draw(texEmptyGlass, r, Color.White);
                    sb.Draw(texPotion, r, glass.FillColor);
                }
                else
                {
                    sb.Draw(texEmptyGlass, r, Color.White);
                }

                string text = glass.IsFilled ? glass.PotionName : "Glass";
                
                Vector2 textSize = font.MeasureString(text);
                float textCenteredX = r.X + (r.Width / 2f) - (textSize.X / 2f);
                
                sb.DrawString(font, text, new Vector2(textCenteredX, r.Bottom + 10), Color.White);
            }
        }

        private void DrawCraftedPotions(SpriteBatch sb)
        {
            Vector2 pos = new Vector2(1650, 120);
            sb.DrawString(font, "Crafted Potions", pos, Color.Gold);

            int y = 150;
            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value <= 0) 
                {
                    continue;
                }
                
                sb.DrawString(font, $"{potion.Key} x{potion.Value}", new Vector2(1650, y), Color.White);
                y += 25;
            }
        }

        private void DrawBin(SpriteBatch sb)
        {
            Rectangle bRect = binBox;
            bRect.X += (int)slideOffset; 
            
            sb.Draw(texTrash, bRect, Color.White);
        }

        private void DrawBox(SpriteBatch sb, Rectangle r, Color color)
        {
            sb.Draw(pixel, r, color);
            sb.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, 2), Color.White);
            sb.Draw(pixel, new Rectangle(r.X, r.Bottom - 2, r.Width, 2), Color.White);
            sb.Draw(pixel, new Rectangle(r.X, r.Y, 2, r.Height), Color.White);
            sb.Draw(pixel, new Rectangle(r.Right - 2, r.Y, 2, r.Height), Color.White);
        }

        private bool LeftPressed()
        {
            return mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
        }

        private bool LeftReleased()
        {
            return mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed;
        }
    }

}
