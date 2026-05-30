using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts;
    public partial class CraftGreyboxSystem
    {
        private void DrawRecipeBookContent(SpriteBatch sb)
{
    
    float baseX = codexPosition.X;
    float baseY = codexPosition.Y;

 
    sb.DrawString(
        font,
        "RECIPE BOOK",
        new Vector2(baseX + 650, baseY + 135),
        Color.Gold
    );

    int yStart = (int)(baseY + 205);

    int leftColX = (int)(baseX + 470);
    int rightColX = (int)(baseX + 1020);

    int lineGap = 34;

    string[] leftRecipes =
    {
        "Lavender + Blue Lotus = Sleep Potion",
        "                                   ",
        
        "Love Rose + Lavender = Love Potion",
        "                                   ",
        
        "Anti-Curse Clover + Sage = Anti-Curse Potion",
        "                                   ",
        
        "Sage + Blue Lotus = Memory Potion",
        "                                   ",
        
        "Red Poppy + Marigold = Pain Relief Potion",
        "                                   ",
        
        "Love Rose + Sage = Persuasion Potion",
        "                                   ",
        
        "Lavender + Anti-Curse Clover = Purification Potion",
        "                                   ",
        
        "Lavender + Sage = Relaxation Potion",
        "                                   ",
    
        "Lavender + Red Poppy = Soothing Potion",
        "                                   "
        
    };

    string[] rightRecipes =
    {

        "Blue Lotus + Love Rose = Mystic Romance Potion",
        "                                   ",
        "Blue Lotus + Anti-Curse Clover = Holy Water Potion",
        "                                   ",
        "Love Rose + Anti-Curse Clover = Heart Protection Potion",
        "                                   ",
        "Love Rose + Red Poppy = Passion Potion",
        "                                   ",
        "Anti-Curse Clover + Red Poppy = Vitality Potion",
        "                                   ",
        "Sage + Red Poppy = Focus Potion",
        "                                   ",
        "Sage + Marigold = Enlightenment Potion",
        "",
        "-- Concentrated Potions --",
        "Lavender + Lavender = Calm Potion",
        "Blue Lotus + Blue Lotus = Clarity Potion",
        "Love Rose + Love Rose = Charm Potion",
        "Anti-Curse Clover + Anti-Curse Clover = Ward Potion",
        "Sage + Sage = Wisdom Potion",
        "Red Poppy + Red Poppy = Rage Potion",
        "Marigold + Marigold = Bright Potion"
    };

    for (int i = 0; i < leftRecipes.Length; i++)
    {
        sb.DrawString(
            font,
            leftRecipes[i],
            new Vector2(leftColX, yStart + i * lineGap),
            Color.DarkCyan
        );
    }

    for (int i = 0; i < rightRecipes.Length; i++)
    {
        Color textColor = rightRecipes[i].StartsWith("--")
            ? Color.Gold
            : Color.DarkCyan;

        sb.DrawString(
            font,
            rightRecipes[i],
            new Vector2(rightColX, yStart + i * lineGap),
            textColor
        );
    }
}
    }