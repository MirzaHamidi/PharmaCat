using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    public partial class CraftGreyboxSystem
    {
        private void DrawRecipeBook(SpriteBatch sb)
        {
            DrawBox(sb, recipeBookPanel, new Color(35, 25, 15, 252));
            sb.DrawString(font, "RECIPE BOOK", new Vector2(recipeBookPanel.X + 50, recipeBookPanel.Y + 35), Color.Gold);

            DrawBox(sb, closeRecipeBookButton, Color.DarkRed);
            sb.DrawString(font, "X", new Vector2(closeRecipeBookButton.X + 16, closeRecipeBookButton.Y + 10), Color.White);

            int yStart = recipeBookPanel.Y + 110;
            int col1X = recipeBookPanel.X + 60;
            int col2X = recipeBookPanel.X + 720;

            string[] col1Recipes = PotionRecipeDatabase.StandardRecipes;
            string[] col2Recipes = PotionRecipeDatabase.ConcentratedRecipes;

            for (int i = 0; i < col1Recipes.Length; i++)
            {
                sb.DrawString(font, col1Recipes[i], new Vector2(col1X, yStart + i * 45), Color.White);
            }

            for (int i = 0; i < col2Recipes.Length; i++)
            {
                Color textColor = col2Recipes[i].StartsWith("---") ? Color.Gold : Color.White;
                sb.DrawString(font, col2Recipes[i], new Vector2(col2X, yStart + i * 45), textColor);
            }
        }

    }
}
