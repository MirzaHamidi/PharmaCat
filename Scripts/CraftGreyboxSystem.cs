using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PharmaCat.Scripts
{
    public class CraftGreyboxSystem
    {
        private Texture2D pixel;
        private SpriteFont font;
        private InventorySystem inventory;

        private MouseState mouse;
        private MouseState oldMouse;

        private List<JarBox> jars = new();
        private List<PotionGlassBox> glasses = new();

        private MortarBox mortar;
        private Rectangle waterBox;
        private Rectangle shopTriggerBox;
        private Rectangle shopPanel;

        private JarBox draggedJar;
        private bool draggingMortar;

        private Random random = new Random();

        private float grindProgress;
        private float grindNeeded = 500f;

        private bool hasWater;
        private bool hasMixedPotion;
        private Color mixedColor;
        private string craftedPotionName = "";
        private Rectangle binBox;
        

        private bool shopOpen;

        public CraftGreyboxSystem(Texture2D pixel, SpriteFont font, InventorySystem inventory)
        {
            this.pixel = pixel;
            this.font = font;
            this.inventory = inventory;
            binBox = new Rectangle(1450, 700, 120, 120);
            mortar = new MortarBox(new Rectangle(430, 260, 220, 140));
            waterBox = new Rectangle(700, 270, 90, 90);

            shopTriggerBox = new Rectangle(0, 980, 1920, 70);
            shopPanel = new Rectangle(250, 760, 900, 200);

            CreateJarsFromInventory();
            CreateGlassesFromInventory();
        }

        public void Update(GameTime gameTime)
        {
            oldMouse = mouse;
            mouse = Mouse.GetState();

            Point mp = mouse.Position;

            shopOpen = shopTriggerBox.Contains(mp) || shopPanel.Contains(mp);
            
            HandleJarDrag(mp);
            HandleWater(mp);
            HandleGrinding(gameTime, mp);
            HandleMortarDrag(mp);
            

            if (shopOpen)
                HandleShopClicks(mp);
        }

        private void CreateJarsFromInventory()
        {
            jars.Clear();

            int index = 0;

            foreach (var herb in inventory.CollectedHerbs)
            {
                if (herb.Value <= 0)
                    continue;

                Rectangle rect = new Rectangle(
                    70 + (index % 6) * 130,
                    120 + (index / 6) * 140,
                    100,
                    115
                );

                jars.Add(new JarBox(
                    herb.Key,
                    GetHerbColor(herb.Key),
                    rect,
                    herb.Value
                ));

                index++;
            }
        }
        
        private void CreateGlassesFromInventory()
        {
            glasses.Clear();

            // YENİ DÜZENLEME: Envanterdeki EmptyBottleCount'a göre ekrana şişeleri dizer.
            for (int i = 0; i < inventory.EmptyBottleCount; i++)
            {
                glasses.Add(new PotionGlassBox(NewGlassPosition(i)));
            }
        }

        private Rectangle NewGlassPosition(int index)
        {
            int x = 430 + (index % 10) * 80;
            int y = 520 + (index / 10) * 115;

            return new Rectangle(x, y, 60, 100);
        }

        private Color GetHerbColor(string herbName)
        {
            switch (herbName)
            {
                case "Lavender":
                    return Color.MediumPurple;
                case "Blue Lotus":
                    return Color.DeepSkyBlue;
                case "Love Rose":
                    return Color.HotPink;
                case "Anti-Curse Clover":
                    return Color.LimeGreen;
                case "Sage":
                    return Color.DarkSeaGreen;
                case "Red Poppy":
                    return Color.Red;
                case "Marigold":
                    return Color.Orange;
                default:
                    return Color.White;
            }
        }

        private string GetPotionResult(string herbA, string herbB)
        {
            string key1 = herbA + "+" + herbB;
            string key2 = herbB + "+" + herbA;

            if (key1 == "Lavender+Blue Lotus" || key2 == "Lavender+Blue Lotus")
                return "Sleep Potion";

            if (key1 == "Love Rose+Lavender" || key2 == "Love Rose+Lavender")
                return "Love Potion";

            if (key1 == "Anti-Curse Clover+Sage" || key2 == "Anti-Curse Clover+Sage")
                return "Anti-Curse Potion";

            if (key1 == "Sage+Blue Lotus" || key2 == "Sage+Blue Lotus")
                return "Memory Potion";

            if (key1 == "Red Poppy+Marigold" || key2 == "Red Poppy+Marigold")
                return "Pain Relief Potion";
            
            if (key1 == "Love Rose+Sage" || key2 == "Love Rose+Sage")
                return "Persuasion Potion";

            return "Unknown Potion";
        }
        
        public void RefreshFromInventory()
        {
            CreateJarsFromInventory();
            CreateGlassesFromInventory();
        }

        private void HandleJarDrag(Point mp)
        {
            if (LeftPressed())
            {
                foreach (var jar in jars)
                {
                    if (jar.Bounds.Contains(mp))
                    {
                        draggedJar = jar;
                        break;
                    }
                }
            }

            if (draggedJar != null && mouse.LeftButton == ButtonState.Pressed)
            {
                draggedJar.DragPosition = new Vector2(
                    mp.X - draggedJar.Bounds.Width / 2,
                    mp.Y - draggedJar.Bounds.Height / 2
                );
            }

            if (draggedJar != null && LeftReleased())
            {
                Rectangle dragRect = new Rectangle(
                    (int)draggedJar.DragPosition.X,
                    (int)draggedJar.DragPosition.Y,
                    draggedJar.Bounds.Width,
                    draggedJar.Bounds.Height
                );

                if (dragRect.Intersects(mortar.Bounds))
                    PourHerb(draggedJar);

                draggedJar.DragPosition = Vector2.Zero;
                draggedJar = null;
            }
        }

        private void PourHerb(JarBox jar)
        {
            if (jar.Amount <= 0)
                return;

            if (!mortar.HasBottom)
            {
                mortar.BottomColor = jar.HerbColor;
                mortar.BottomHerbName = jar.Name;
                mortar.HasBottom = true;

                inventory.RemoveHerb(jar.Name, 1);
                CreateJarsFromInventory();
            }
            else if (!mortar.HasTop)
            {
                mortar.TopColor = jar.HerbColor;
                mortar.TopHerbName = jar.Name;
                mortar.HasTop = true;

                inventory.RemoveHerb(jar.Name, 1);
                CreateJarsFromInventory();
            }
        }

        private void HandleWater(Point mp)
        {
            if (!LeftPressed())
                return;

            if (!waterBox.Contains(mp))
                return;

            if (!mortar.HasBottom || !mortar.HasTop)
                return;

            hasWater = true;
        }

        private void HandleGrinding(GameTime gameTime, Point mp)
        {
            if (!mortar.HasBottom || !mortar.HasTop || !hasWater)
                return;

            if (mouse.LeftButton == ButtonState.Pressed && mortar.Grinder.Contains(mp))
            {
                float grindSpeed = 70f + inventory.MortarLevel * 35f;
                grindProgress += grindSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (grindProgress >= grindNeeded)
                {
                    mixedColor = Mix(mortar.BottomColor, mortar.TopColor);
                    hasMixedPotion = true;

                    craftedPotionName = GetPotionResult(
                        mortar.BottomHerbName,
                        mortar.TopHerbName
                    );

                    mortar.HasBottom = false;
                    mortar.HasTop = false;
                    mortar.BottomHerbName = "";
                    mortar.TopHerbName = "";

                    grindProgress = 0f;
                }
            }
        }

        private void HandleMortarDrag(Point mp)
        {
            if (LeftPressed() && mortar.Bounds.Contains(mp) && hasMixedPotion)
                draggingMortar = true;

            if (draggingMortar && LeftReleased())
            {
                if (binBox.Contains(mp))
                {
                    hasMixedPotion = false;
                    hasWater = false;
                    mixedColor = Color.Transparent;
                    craftedPotionName = "";
                    grindProgress = 0f;

                    draggingMortar = false;
                    return;
                }

                foreach (var glass in glasses)
                {
                    if (glass.Bounds.Contains(mp) && !glass.IsFilled)
                    {
                        glass.IsFilled = true;
                        glass.FillColor = mixedColor;
                        glass.PotionName = craftedPotionName;

                        inventory.AddPotion(craftedPotionName, 1);

                        // Şişeyi doldurunca envanterden boş şişe hakkını düşürür
                        if (inventory.EmptyBottleCount > 0)
                            inventory.EmptyBottleCount--;

                        // Bu satırı KALDIRDIK. Doldurulan şişeler ekrandan yok olmasın diye.
                        // CreateGlassesFromInventory();

                        hasMixedPotion = false;
                        hasWater = false;
                        mixedColor = Color.Transparent;
                        craftedPotionName = "";

                        break;
                    }
                }

                draggingMortar = false;
            }
        }
        
        private void HandleShopClicks(Point mp)
        {
            if (!LeftPressed())
                return;

            string[] herbNames =
            {
                "Lavender",
                "Blue Lotus",
                "Love Rose",
                "Anti-Curse Clover",
                "Sage",
                "Red Poppy",
                "Marigold"
            };

            for (int i = 0; i < herbNames.Length; i++)
            {
                Rectangle herbButton = new Rectangle(
                    shopPanel.X + 30 + (i % 4) * 190,
                    shopPanel.Y + 55 + (i / 4) * 55, 170,45);

                if (herbButton.Contains(mp) && inventory.SpendMoney(15))
                {
                    inventory.AddHerb(herbNames[i], 1);
                    CreateJarsFromInventory();
                }
            }

            Rectangle buyBottle = new Rectangle(
                shopPanel.X + 30,
                shopPanel.Y + 170, 170, 45);

            Rectangle buyMortar = new Rectangle(
                shopPanel.X + 240,
                shopPanel.Y + 170, 220, 45);

            if (buyBottle.Contains(mp) && inventory.SpendMoney(25))
            {
                inventory.EmptyBottleCount++;
                CreateGlassesFromInventory();
            }

            if (buyMortar.Contains(mp) && inventory.SpendMoney(80))
            {
                inventory.MortarLevel++;
            }
        }

        private Color Mix(Color a, Color b)
        {
            return new Color(
                (a.R + b.R) / 2,
                (a.G + b.G) / 2,
                (a.B + b.B) / 2
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawMoney(spriteBatch);
            DrawShopTrigger(spriteBatch);
            DrawJars(spriteBatch);
            DrawMortar(spriteBatch);
            DrawWater(spriteBatch);
            DrawGlasses(spriteBatch);
            DrawCraftedPotions(spriteBatch);
            DrawBin(spriteBatch);
            
            if (shopOpen)
                DrawShop(spriteBatch);

            if (draggingMortar && hasMixedPotion)
            {
                spriteBatch.Draw(pixel, new Rectangle(mouse.X - 30, mouse.Y - 30, 60, 60), mixedColor);
                spriteBatch.DrawString(font, craftedPotionName, new Vector2(mouse.X + 35, mouse.Y - 10), Color.White);
            }
        }

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

                if (draggedJar == jar)
                    r = new Rectangle((int)jar.DragPosition.X, (int)jar.DragPosition.Y, r.Width, r.Height);

                DrawBox(sb, r, Color.DarkSlateGray);

                Rectangle inner = new Rectangle(r.X + 18, r.Y + 28, r.Width - 36, r.Height - 45);
                sb.Draw(pixel, inner, jar.HerbColor);

                sb.DrawString(font, jar.Name, new Vector2(r.X, r.Y - 20), Color.White);
                sb.DrawString(font, "x" + jar.Amount, new Vector2(r.X + 32, r.Bottom - 24), Color.White);
            }
        }

        private void DrawMortar(SpriteBatch sb)
        {
            DrawBox(sb, mortar.Bounds, Color.Gray);

            Rectangle bowl = new Rectangle(
                mortar.Bounds.X + 35,
                mortar.Bounds.Y + 30,
                mortar.Bounds.Width - 70,
                mortar.Bounds.Height - 60
            );

            DrawBox(sb, bowl, new Color(255, 255, 255, 80));

            Rectangle top = new Rectangle(bowl.X, bowl.Y, bowl.Width, bowl.Height / 2);
            Rectangle bottom = new Rectangle(bowl.X, bowl.Y + bowl.Height / 2, bowl.Width, bowl.Height / 2);

            if (mortar.HasBottom)
                sb.Draw(pixel, bottom, mortar.BottomColor);

            if (mortar.HasTop)
                sb.Draw(pixel, top, mortar.TopColor);

            if (hasMixedPotion)
                sb.Draw(pixel, bowl, mixedColor);

            if (hasWater)
                sb.Draw(pixel, bowl, new Color(80, 170, 255, 80));

            DrawBox(sb, mortar.Grinder, Color.SaddleBrown);

            sb.DrawString(font, "MORTAR", new Vector2(mortar.Bounds.X, mortar.Bounds.Y - 24), Color.White);
            sb.DrawString(font, "1) Drag herbs  2) Add water  3) Hold pestle  4) Drag mortar to glass",
                new Vector2(mortar.Bounds.X - 120, mortar.Bounds.Bottom + 8), Color.White);

            Rectangle barBack = new Rectangle(mortar.Bounds.X, mortar.Bounds.Bottom + 34, mortar.Bounds.Width, 12);
            Rectangle barFill = new Rectangle(
                barBack.X,
                barBack.Y,
                (int)(barBack.Width * (grindProgress / grindNeeded)),
                barBack.Height
            );

            sb.Draw(pixel, barBack, Color.DarkRed);
            sb.Draw(pixel, barFill, Color.LimeGreen);

            if (!string.IsNullOrEmpty(craftedPotionName) && hasMixedPotion)
            {
                sb.DrawString(font, craftedPotionName, new Vector2(mortar.Bounds.X, mortar.Bounds.Bottom + 52), Color.Gold);
            }
        }

        private void DrawWater(SpriteBatch sb)
        {
            DrawBox(sb, waterBox, Color.CornflowerBlue);
            sb.DrawString(font, "Water", new Vector2(waterBox.X + 8, waterBox.Y - 22), Color.White);
        }

        private void DrawGlasses(SpriteBatch sb)
        {
            foreach (var glass in glasses)
            {
                DrawBox(sb, glass.Bounds, Color.LightGray);

                Rectangle fill = new Rectangle(
                    glass.Bounds.X + 10,
                    glass.Bounds.Y + 35,
                    glass.Bounds.Width - 20,
                    glass.Bounds.Height - 45
                );

                if (glass.IsFilled)
                    sb.Draw(pixel, fill, glass.FillColor);

                string text = glass.IsFilled ? glass.PotionName : "Glass";
                sb.DrawString(font, text, new Vector2(glass.Bounds.X - 10, glass.Bounds.Bottom + 5), Color.White);
            }
        }

        private void DrawCraftedPotions(SpriteBatch sb)
        {
            Vector2 pos = new Vector2(1250, 120);
            sb.DrawString(font, "Crafted Potions", pos, Color.Gold);

            int y = 150;

            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value <= 0)
                    continue;

                sb.DrawString(font, $"{potion.Key} x{potion.Value}", new Vector2(1250, y), Color.White);
                y += 25;
            }
        }

        private void DrawBin(SpriteBatch sb)
        {
            DrawBox(sb, binBox, Color.DarkRed);
            sb.DrawString(font, "TRASH", new Vector2(binBox.X + 25, binBox.Y + 45), Color.White);
        }

        private void DrawShopTrigger(SpriteBatch sb)
        {
            sb.Draw(pixel, shopTriggerBox, new Color(60, 60, 60, 180));
            sb.DrawString(font, "Move mouse here to open shop", new Vector2(760, shopTriggerBox.Y + 22), Color.White);
        }

        private void DrawShop(SpriteBatch sb)
        {
            DrawBox(sb, shopPanel, new Color(25, 25, 25, 240));

            sb.DrawString(font, "SHOP", new Vector2(shopPanel.X + 20, shopPanel.Y + 15), Color.Gold);

            string[] herbNames =
            {
                "Lavender",
                "Blue Lotus",
                "Love Rose",
                "Anti-Curse Clover",
                "Sage",
                "Red Poppy",
                "Marigold"
            };

            for (int i = 0; i < herbNames.Length; i++)
            {
                Rectangle herbButton = new Rectangle(
                    shopPanel.X + 30 + (i % 4) * 190,
                    shopPanel.Y + 55 + (i / 4) * 55,
                    170,
                    45
                );

                DrawShopButton(
                    sb,
                    herbButton,
                    herbNames[i] + " $15"
                );
            }

            DrawShopButton(
                sb,
                new Rectangle(shopPanel.X + 30, shopPanel.Y + 170, 170, 45),
                "Bottle $25"
            );

            DrawShopButton(
                sb,
                new Rectangle(shopPanel.X + 240, shopPanel.Y + 170, 220, 45),
                "Upgrade Mortar $80"
            );

            
        }

        private void DrawShopButton(SpriteBatch sb, Rectangle r, string text)
        {
            DrawBox(sb, r, Color.DarkOliveGreen);
            sb.DrawString(font, text, new Vector2(r.X + 8, r.Y + 13), Color.White);
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
            return mouse.LeftButton == ButtonState.Pressed &&
                   oldMouse.LeftButton == ButtonState.Released;
        }

        private bool LeftReleased()
        {
            return mouse.LeftButton == ButtonState.Released &&
                   oldMouse.LeftButton == ButtonState.Pressed;
        }
    }

    public class JarBox
    {
        public string Name;
        public Color HerbColor;
        public Rectangle Bounds;
        public Vector2 DragPosition;
        public int Amount;

        public JarBox(string name, Color herbColor, Rectangle bounds, int amount)
        {
            Name = name;
            HerbColor = herbColor;
            Bounds = bounds;
            Amount = amount;
        }
    }

    public class PotionGlassBox
    {
        public string PotionName;
        public Rectangle Bounds;
        public bool IsFilled;
        public Color FillColor;

        public PotionGlassBox(Rectangle bounds)
        {
            Bounds = bounds;
            PotionName = "";
        }
    }

    public class MortarBox
    {
        public Rectangle Bounds;
        public Rectangle Grinder;

        public string BottomHerbName;
        public string TopHerbName;

        public bool HasBottom;
        public bool HasTop;

        public Color BottomColor;
        public Color TopColor;

        public MortarBox(Rectangle bounds)
        {
            Bounds = bounds;
            Grinder = new Rectangle(bounds.Right + 25, bounds.Y - 15, 35, bounds.Height + 65);

            BottomHerbName = "";
            TopHerbName = "";
        }
    }
}