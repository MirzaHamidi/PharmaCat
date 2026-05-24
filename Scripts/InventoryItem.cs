using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    public class InventoryItem
    {
        public string Name;
        public ItemType Type;
        public int Amount;

        // ASSET GELİNCE BURAYA ICON EKLENECEK
        public Texture2D IconTexture;

        public InventoryItem(string name, ItemType type, int amount)
        {
            Name = name;
            Type = type;
            Amount = amount;
        }
    }
}