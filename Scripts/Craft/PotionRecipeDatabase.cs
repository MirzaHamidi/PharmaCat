using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public static class PotionRecipeDatabase
    {
        public static readonly string[] StandardRecipes =
        {
            "Lavender + Blue Lotus = Sleep Potion",
            "Love Rose + Lavender = Love Potion",
            "Anti-Curse Clover + Sage = Anti-Curse Potion",
            "Sage + Blue Lotus = Memory Potion",
            "Red Poppy + Marigold = Pain Relief Potion",
            "Love Rose + Sage = Persuasion Potion",
            "Lavender + Anti-Curse Clover = Purification Potion",
            "Lavender + Sage = Relaxation Potion",
            "Lavender + Red Poppy = Soothing Potion",
            "Blue Lotus + Love Rose = Mystic Romance Potion",
            "Blue Lotus + Anti-Curse Clover = Holy Water Potion",
            "Love Rose + Anti-Curse Clover = Heart Protection Potion"
        };

        public static readonly string[] ConcentratedRecipes =
        {
            "Love Rose + Red Poppy = Passion Potion",
            "Anti-Curse Clover + Red Poppy = Vitality Potion",
            "Sage + Red Poppy = Focus Potion",
            "Sage + Marigold = Enlightenment Potion",
            "",
            "--- CONCENTRATED POTIONS ---",
            "Lavender + Lavender = Calm Potion",
            "Blue Lotus + Blue Lotus = Clarity Potion",
            "Love Rose + Love Rose = Charm Potion",
            "Anti-Curse Clover + Anti-Curse Clover = Ward Potion",
            "Sage + Sage = Wisdom Potion",
            "Red Poppy + Red Poppy = Rage Potion",
            "Marigold + Marigold = Bright Potion"
        };

        private static readonly Dictionary<string, string> Recipes = new Dictionary<string, string>
        {
            { Key("Lavender", "Blue Lotus"), "Sleep Potion" },
            { Key("Love Rose", "Lavender"), "Love Potion" },
            { Key("Anti-Curse Clover", "Sage"), "Anti-Curse Potion" },
            { Key("Sage", "Blue Lotus"), "Memory Potion" },
            { Key("Red Poppy", "Marigold"), "Pain Relief Potion" },
            { Key("Love Rose", "Sage"), "Persuasion Potion" },
            { Key("Lavender", "Anti-Curse Clover"), "Purification Potion" },
            { Key("Lavender", "Sage"), "Relaxation Potion" },
            { Key("Lavender", "Red Poppy"), "Soothing Potion" },
            { Key("Blue Lotus", "Love Rose"), "Mystic Romance Potion" },
            { Key("Blue Lotus", "Anti-Curse Clover"), "Holy Water Potion" },
            { Key("Love Rose", "Anti-Curse Clover"), "Heart Protection Potion" },
            { Key("Love Rose", "Red Poppy"), "Passion Potion" },
            { Key("Anti-Curse Clover", "Red Poppy"), "Vitality Potion" },
            { Key("Sage", "Red Poppy"), "Focus Potion" },
            { Key("Sage", "Marigold"), "Enlightenment Potion" },
            { Key("Lavender", "Lavender"), "Calm Potion" },
            { Key("Blue Lotus", "Blue Lotus"), "Clarity Potion" },
            { Key("Love Rose", "Love Rose"), "Charm Potion" },
            { Key("Anti-Curse Clover", "Anti-Curse Clover"), "Ward Potion" },
            { Key("Sage", "Sage"), "Wisdom Potion" },
            { Key("Red Poppy", "Red Poppy"), "Rage Potion" },
            { Key("Marigold", "Marigold"), "Bright Potion" }
        };

        public static string GetPotionResult(string herbA, string herbB)
        {
            string key = Key(herbA, herbB);
            return Recipes.TryGetValue(key, out string result) ? result : "Unknown Potion";
        }

        private static string Key(string herbA, string herbB)
        {
            if (string.CompareOrdinal(herbA, herbB) <= 0)
                return herbA + "+" + herbB;

            return herbB + "+" + herbA;
        }
    }
}
