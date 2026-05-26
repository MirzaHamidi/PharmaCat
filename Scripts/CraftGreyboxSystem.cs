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
        private Rectangle binBox;
        
        // UI Butonları ve Panelleri
        private bool shopOpen;
        private bool recipeBookOpen;
        
        private Rectangle recipeBookButton;
        private Rectangle recipeBookPanel;
        private Rectangle closeRecipeBookButton;
        
        private Rectangle shopButton;
        private Rectangle shopPanel;
        private Rectangle closeShopButton;

        private JarBox draggedJar;
        private bool draggingMortar;

        private Random random = new Random();

        private float grindProgress;
        private float grindNeeded = 500f;

        private bool hasWater;
        private bool hasMixedPotion;
        private Color mixedColor;
        private string craftedPotionName = "";

        public CraftGreyboxSystem(Texture2D pixel, SpriteFont font, InventorySystem inventory)
        {
            this.pixel = pixel;
            this.font = font;
            this.inventory = inventory;
            
            // UX DÜZENLEMESİ: Koordinatlar Montaj Hattı mantığına göre baştan ayarlandı
            
            // Çöp Kutusu (Sağ alt köşe, oyun alanından uzak)
            binBox = new Rectangle(1650, 800, 120, 120);
            
            // Havan (Ekranın tam ortası, çalışma alanı)
            mortar = new MortarBox(new Rectangle(750, 380, 250, 160));
            
            // Su (Havan çubuğuna çarpmaması için güvenli bir mesafede, sağda)
            waterBox = new Rectangle(1100, 410, 90, 90); 

            // Tarif defteri buton ve paneli (Sağ üst)
            recipeBookButton = new Rectangle(1650, 30, 230, 55);
            recipeBookPanel = new Rectangle(260, 50, 1400, 920);
            closeRecipeBookButton = new Rectangle(1580, 70, 50, 45);

            // Dükkan (Shop) buton ve paneli (Tarif defterinin yanı)
            shopButton = new Rectangle(1400, 30, 230, 55); 
            shopPanel = new Rectangle(460, 240, 1000, 400); 
            closeShopButton = new Rectangle(1400, 260, 40, 40); 

            CreateJarsFromInventory();
            CreateGlassesFromInventory();
        }

        public void Update(GameTime gameTime)
        {
            oldMouse = mouse;
            mouse = Mouse.GetState();

            Point mp = mouse.Position;

            // 1. ÖNCELİK: Tarif defteri açıksa
            if (recipeBookOpen)
            {
                if (LeftPressed() && closeRecipeBookButton.Contains(mp))
                {
                    recipeBookOpen = false;
                }
                return; 
            }

            // 2. ÖNCELİK: Dükkan açıksa
            if (shopOpen)
            {
                if (LeftPressed() && closeShopButton.Contains(mp))
                {
                    shopOpen = false;
                }
                
                HandleShopClicks(mp);
                return; 
            }

            // Arayüz butonlarını açma kontrolü
            if (LeftPressed() && recipeBookButton.Contains(mp))
            {
                recipeBookOpen = true;
                return;
            }

            if (LeftPressed() && shopButton.Contains(mp))
            {
                shopOpen = true;
                return;
            }
            
            // Eğer menüler kapalıysa oyun mekanikleri çalışır
            HandleJarDrag(mp);
            HandleWater(mp);
            HandleGrinding(gameTime, mp);
            HandleMortarDrag(mp);
        }

        private void CreateJarsFromInventory()
        {
            jars.Clear();
            int index = 0;

            foreach (var herb in inventory.CollectedHerbs)
            {
                if (herb.Value <= 0)
                    continue;

                // UX DÜZENLEMESİ: Bitkiler ekranın üst kısmına, temiz bir raf gibi dizildi
                Rectangle rect = new Rectangle(
                    150 + (index % 7) * 150, // 7 bitki yan yana sığacak genişlik
                    100,                     // Sabit yükseklik, masadan yukarıda
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

            for (int i = 0; i < inventory.EmptyBottleCount; i++)
            {
                glasses.Add(new PotionGlassBox(NewGlassPosition(i)));
            }
        }

        private Rectangle NewGlassPosition(int index)
        {
            // UX DÜZENLEMESİ: Şişeler havanın çok daha altına (Y: 750) ve ortalanarak yerleştirildi
            int x = 600 + (index % 6) * 160; 
            int y = 750 + (index / 6) * 160;

            return new Rectangle(x, y, 60, 100);
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
                {
                    PourHerb(draggedJar);
                }

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
            if (!LeftPressed()) return;
            if (!waterBox.Contains(mp)) return;
            if (!mortar.HasBottom || !mortar.HasTop) return;

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

                    craftedPotionName = GetPotionResult(mortar.BottomHerbName, mortar.TopHerbName);

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
            {
                draggingMortar = true;
            }

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

                        if (inventory.EmptyBottleCount > 0)
                        {
                            inventory.EmptyBottleCount--;
                        }

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

            string[] herbNames = { "Lavender", "Blue Lotus", "Love Rose", "Anti-Curse Clover", "Sage", "Red Poppy", "Marigold" };

            // Bitki Satın Alma Butonları Kontrolü
            for (int i = 0; i < herbNames.Length; i++)
            {
                Rectangle herbButton = new Rectangle(shopPanel.X + 40 + (i % 4) * 230, shopPanel.Y + 90 + (i / 4) * 80, 210, 55);
                
                if (herbButton.Contains(mp) && inventory.SpendMoney(15))
                {
                    inventory.AddHerb(herbNames[i], 1);
                    CreateJarsFromInventory();
                }
            }

            // Şişe Satın Alma
            Rectangle buyBottle = new Rectangle(shopPanel.X + 40, shopPanel.Y + 280, 210, 55);
            if (buyBottle.Contains(mp) && inventory.SpendMoney(25))
            {
                inventory.EmptyBottleCount++;
                CreateGlassesFromInventory();
            }

            // Havan (Mortar) Yükseltme
            Rectangle buyMortar = new Rectangle(shopPanel.X + 270, shopPanel.Y + 280, 250, 55);
            if (buyMortar.Contains(mp) && inventory.SpendMoney(80))
            {
                inventory.MortarLevel++;
            }
        }

        private Color GetHerbColor(string herbName)
        {
            switch (herbName)
            {
                case "Lavender": return Color.MediumPurple;
                case "Blue Lotus": return Color.DeepSkyBlue;
                case "Love Rose": return Color.HotPink;
                case "Anti-Curse Clover": return Color.LimeGreen;
                case "Sage": return Color.DarkSeaGreen;
                case "Red Poppy": return Color.Red;
                case "Marigold": return Color.Orange;
                default: return Color.White;
            }
        }

        private string GetPotionResult(string herbA, string herbB)
        {
            string key1 = herbA + "+" + herbB;
            string key2 = herbB + "+" + herbA;

            if (key1 == "Lavender+Blue Lotus" || key2 == "Lavender+Blue Lotus") return "Sleep Potion";
            if (key1 == "Love Rose+Lavender" || key2 == "Love Rose+Lavender") return "Love Potion";
            if (key1 == "Anti-Curse Clover+Sage" || key2 == "Anti-Curse Clover+Sage") return "Anti-Curse Potion";
            if (key1 == "Sage+Blue Lotus" || key2 == "Sage+Blue Lotus") return "Memory Potion";
            if (key1 == "Red Poppy+Marigold" || key2 == "Red Poppy+Marigold") return "Pain Relief Potion";
            if (key1 == "Love Rose+Sage" || key2 == "Love Rose+Sage") return "Persuasion Potion";
            if (key1 == "Lavender+Anti-Curse Clover" || key2 == "Lavender+Anti-Curse Clover") return "Purification Potion";
            if (key1 == "Lavender+Sage" || key2 == "Lavender+Sage") return "Relaxation Potion";
            if (key1 == "Lavender+Red Poppy" || key2 == "Lavender+Red Poppy") return "Soothing Potion";
            if (key1 == "Blue Lotus+Love Rose" || key2 == "Blue Lotus+Love Rose") return "Mystic Romance Potion";
            if (key1 == "Blue Lotus+Anti-Curse Clover" || key2 == "Blue Lotus+Anti-Curse Clover") return "Holy Water Potion";
            if (key1 == "Love Rose+Anti-Curse Clover" || key2 == "Love Rose+Anti-Curse Clover") return "Heart Protection Potion";
            if (key1 == "Love Rose+Red Poppy" || key2 == "Love Rose+Red Poppy") return "Passion Potion";
            if (key1 == "Anti-Curse Clover+Red Poppy" || key2 == "Anti-Curse Clover+Red Poppy") return "Vitality Potion";
            if (key1 == "Sage+Red Poppy" || key2 == "Sage+Red Poppy") return "Focus Potion";
            if (key1 == "Sage+Marigold" || key2 == "Sage+Marigold") return "Enlightenment Potion";

            if (key1 == "Lavender+Lavender") return "Calm Potion";
            if (key1 == "Blue Lotus+Blue Lotus") return "Clarity Potion";
            if (key1 == "Love Rose+Love Rose") return "Charm Potion";
            if (key1 == "Anti-Curse Clover+Anti-Curse Clover") return "Ward Potion";
            if (key1 == "Sage+Sage") return "Wisdom Potion";
            if (key1 == "Red Poppy+Red Poppy") return "Rage Potion";
            if (key1 == "Marigold+Marigold") return "Bright Potion";

            return "Unknown Potion";
        }

        private Color Mix(Color a, Color b)
        {
            return new Color((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawMoney(spriteBatch);
            DrawJars(spriteBatch);
            DrawMortar(spriteBatch);
            DrawWater(spriteBatch);
            DrawGlasses(spriteBatch);
            DrawCraftedPotions(spriteBatch);
            DrawBin(spriteBatch);
            
            // Arayüz Menü Butonları
            DrawBox(spriteBatch, recipeBookButton, Color.DarkSlateGray);
            spriteBatch.DrawString(font, "Recipe Book", new Vector2(recipeBookButton.X + 35, recipeBookButton.Y + 15), Color.White);

            DrawBox(spriteBatch, shopButton, Color.DarkOliveGreen);
            spriteBatch.DrawString(font, "Open Shop", new Vector2(shopButton.X + 45, shopButton.Y + 15), Color.White);

            // Fareye yapışık sürüklenen iksir
            if (draggingMortar && hasMixedPotion)
            {
                spriteBatch.Draw(pixel, new Rectangle(mouse.X - 30, mouse.Y - 30, 60, 60), mixedColor);
                spriteBatch.DrawString(font, craftedPotionName, new Vector2(mouse.X + 35, mouse.Y - 10), Color.White);
            }

            // Paneller (Eğer açıklarsa)
            if (shopOpen)
            {
                DrawShop(spriteBatch);
            }

            if (recipeBookOpen)
            {
                DrawRecipeBook(spriteBatch);
            }
        }

        private void DrawRecipeBook(SpriteBatch sb)
        {
            DrawBox(sb, recipeBookPanel, new Color(35, 25, 15, 252)); 
            sb.DrawString(font, "RECIPE BOOK (TARIF DEFTERI)", new Vector2(recipeBookPanel.X + 50, recipeBookPanel.Y + 35), Color.Gold);
            
            DrawBox(sb, closeRecipeBookButton, Color.DarkRed);
            sb.DrawString(font, "X", new Vector2(closeRecipeBookButton.X + 16, closeRecipeBookButton.Y + 10), Color.White);

            int yStart = recipeBookPanel.Y + 110;
            int col1X = recipeBookPanel.X + 60;
            int col2X = recipeBookPanel.X + 720;
            
            string[] col1Recipes = {
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

            string[] col2Recipes = {
                "Love Rose + Red Poppy = Passion Potion",
                "Anti-Curse Clover + Red Poppy = Vitality Potion",
                "Sage + Red Poppy = Focus Potion",
                "Sage + Marigold = Enlightenment Potion",
                "",
                "--- CONCENTRATED POTIONS (AYNI TUR BITKILER) ---",
                "Lavender + Lavender = Calm Potion",
                "Blue Lotus + Blue Lotus = Clarity Potion",
                "Love Rose + Love Rose = Charm Potion",
                "Anti-Curse Clover + Anti-Curse Clover = Ward Potion",
                "Sage + Sage = Wisdom Potion",
                "Red Poppy + Red Poppy = Rage Potion",
                "Marigold + Marigold = Bright Potion"
            };

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

        private void DrawShop(SpriteBatch sb)
        {
            DrawBox(sb, shopPanel, new Color(25, 25, 25, 250));
            sb.DrawString(font, "MERCHANT SHOP", new Vector2(shopPanel.X + 40, shopPanel.Y + 30), Color.Gold);

            DrawBox(sb, closeShopButton, Color.DarkRed);
            sb.DrawString(font, "X", new Vector2(closeShopButton.X + 11, closeShopButton.Y + 8), Color.White);

            string[] herbNames = { "Lavender", "Blue Lotus", "Love Rose", "Anti-Curse Clover", "Sage", "Red Poppy", "Marigold" };

            // Dükkandaki ürünlerin butonlarını çiz
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
                {
                    r = new Rectangle((int)jar.DragPosition.X, (int)jar.DragPosition.Y, r.Width, r.Height);
                }

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

            Rectangle bowl = new Rectangle(mortar.Bounds.X + 35, mortar.Bounds.Y + 30, mortar.Bounds.Width - 70, mortar.Bounds.Height - 60);
            DrawBox(sb, bowl, new Color(255, 255, 255, 80));

            Rectangle top = new Rectangle(bowl.X, bowl.Y, bowl.Width, bowl.Height / 2);
            Rectangle bottom = new Rectangle(bowl.X, bowl.Y + bowl.Height / 2, bowl.Width, bowl.Height / 2);

            if (mortar.HasBottom) sb.Draw(pixel, bottom, mortar.BottomColor);
            if (mortar.HasTop) sb.Draw(pixel, top, mortar.TopColor);
            if (hasMixedPotion) sb.Draw(pixel, bowl, mixedColor);
            if (hasWater) sb.Draw(pixel, bowl, new Color(80, 170, 255, 80));

            DrawBox(sb, mortar.Grinder, Color.SaddleBrown);

            sb.DrawString(font, "MORTAR", new Vector2(mortar.Bounds.X, mortar.Bounds.Y - 24), Color.White);
            sb.DrawString(font, "1) Drag herbs  2) Add water  3) Hold pestle  4) Drag mortar to glass", new Vector2(mortar.Bounds.X - 120, mortar.Bounds.Bottom + 8), Color.White);

            Rectangle barBack = new Rectangle(mortar.Bounds.X, mortar.Bounds.Bottom + 34, mortar.Bounds.Width, 12);
            Rectangle barFill = new Rectangle(barBack.X, barBack.Y, (int)(barBack.Width * (grindProgress / grindNeeded)), barBack.Height);

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

                Rectangle fill = new Rectangle(glass.Bounds.X + 10, glass.Bounds.Y + 35, glass.Bounds.Width - 20, glass.Bounds.Height - 45);

                if (glass.IsFilled)
                {
                    sb.Draw(pixel, fill, glass.FillColor);
                }

                string text = glass.IsFilled ? glass.PotionName : "Glass";
                
                // Şişe yazısını tam ortala
                Vector2 textSize = font.MeasureString(text);
                float textCenteredX = glass.Bounds.X + (glass.Bounds.Width / 2f) - (textSize.X / 2f);
                
                sb.DrawString(font, text, new Vector2(textCenteredX, glass.Bounds.Bottom + 10), Color.White);
            }
        }

        private void DrawCraftedPotions(SpriteBatch sb)
        {
            Vector2 pos = new Vector2(1650, 120);
            sb.DrawString(font, "Crafted Potions", pos, Color.Gold);

            int y = 150;
            foreach (var potion in inventory.CraftedPotions)
            {
                if (potion.Value <= 0) continue;
                sb.DrawString(font, $"{potion.Key} x{potion.Value}", new Vector2(1650, y), Color.White);
                y += 25;
            }
        }

        private void DrawBin(SpriteBatch sb)
        {
            DrawBox(sb, binBox, Color.DarkRed);
            sb.DrawString(font, "TRASH", new Vector2(binBox.X + 25, binBox.Y + 45), Color.White);
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