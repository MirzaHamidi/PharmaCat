using System.Collections.Generic;

namespace PharmaCat.Scripts
{
    public class InventorySystem
    {
        public int Money = 150;
        public int MortarLevel = 1;
        public int EmptyBottleCount = 3;

        public Dictionary<string, int> CollectedHerbs = new Dictionary<string, int>()
        {
            { "Lavender", 0 },
            { "Blue Lotus", 0 },
            { "Love Rose", 0 },
            { "Anti-Curse Clover", 0 },
            { "Sage", 0 },
            { "Red Poppy", 0 },
            { "Marigold", 0 }
        };

        public Dictionary<string, int> CraftedPotions = new Dictionary<string, int>();

        public void AddHerb(string herbName, int amount)
        {
            if (!CollectedHerbs.ContainsKey(herbName))
                CollectedHerbs.Add(herbName, 0);

            CollectedHerbs[herbName] += amount;
        }

        public bool RemoveHerb(string herbName, int amount)
        {
            if (!CollectedHerbs.ContainsKey(herbName))
                return false;

            if (CollectedHerbs[herbName] < amount)
                return false;

            CollectedHerbs[herbName] -= amount;
            return true;
        }

        public void AddPotion(string potionName, int amount)
        {
            if (!CraftedPotions.ContainsKey(potionName))
                CraftedPotions.Add(potionName, 0);

            CraftedPotions[potionName] += amount;
        }

        public bool RemovePotion(string potionName, int amount)
        {
            if (!CraftedPotions.ContainsKey(potionName))
                return false;

            if (CraftedPotions[potionName] < amount)
                return false;

            CraftedPotions[potionName] -= amount;
            return true;
        }

        public bool SpendMoney(int price)
        {
            if (Money < price)
                return false;

            Money -= price;
            return true;
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }
    }
}