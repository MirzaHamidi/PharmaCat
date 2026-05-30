using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    public class CraftGreyboxSystem
    {
        private Texture2D pixel;
        private SpriteFont font;
        private InventorySystem inventory;

        private Texture2D texLavender, texBlueLotus, texAntiCurse, texSage;
        private Texture2D texLoveRose, texRedPoppy, texMarigold;
        private Texture2D texEmptyGlass, texPotion, texMortar1, texMortar, texGrinder, texTrash, texTable, texWall, texLight, texPanjur, texBooktab, texMortarDust, texMortarDust1, texPanjurip;

        private Dictionary<string, Texture2D> texCustBase = new Dictionary<string, Texture2D>();
        private Dictionary<string, Texture2D> texCustHappy = new Dictionary<string, Texture2D>();
        private Dictionary<string, Texture2D> texCustAngry = new Dictionary<string, Texture2D>();

        private MouseState mouse;
        private MouseState oldMouse;

        private List<JarBox> jars = new List<JarBox>();
        private List<PotionGlassBox> glasses = new List<PotionGlassBox>();

        private MortarBox mortar;
        private Rectangle waterBox;
        private Rectangle binBox;
       
        private Rectangle mortarGrindArea;
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
        private bool draggingGrinder;
        private Vector2 grinderDragOffset;
        private bool grinderWasInsideMortar;
        private Rectangle grinderCurrentRect;

        private Random random = new Random();

        private float grindProgress;
        private float grindNeeded = 500f;
    
        private bool hasWater;
        private bool hasMixedPotion;
        private Color mixedColor;
        private string craftedPotionName = "";
        
        private float mortarScale = 0.3f;
        private float grinderScale = 0.3f;
        
        private Rectangle panjurButton;
        private int ropeButtonOffsetX = 400;
        private int ropeButtonOffsetY = 0;
        private int ropeButtonWidth = 160;
        private int ropeButtonHeight = 220;
        private Rectangle panjurRopeButton;

        private Vector2 panjurPosition;
        private Vector2 panjurRopePosition;

        private bool panjurOpening;
        private bool panjurClosing;

        private bool ropeComingDown;
        private bool ropeGoingUp;

        private bool panjurButtonVisible = true;
        private bool ropeButtonActive = false;
        private bool ropeVisible = false;

        private float panjurSpeed = 120f;
        private float ropeSpeed = 180f;

        private float panjurClosedY = 0f;
        private float panjurOpenY = -750f;

        private float ropeHiddenY = -300f;
        private float ropeDownY = 0f;

        public Action<PotionGlassBox> OnSellAttempt;
        public Action OnShopFinished;
        private PotionGlassBox draggedGlass;
        
        private Customers currentCustomer;
        private string currentCharacter;
        private enum CustomerEmotion { Silhouette, Neutral, Happy, Angry }
        private CustomerEmotion currentEmotion;
        
        private Rectangle customerRect = new Rectangle(1920 / 2 - 250, 240, 500, 600);
        
        private float slideOffset = 0f;
        private bool itemsSlidingOut = false;

        private int haggleAttempts = 0;
        private bool isCustomerLeaving = false;
        private bool isCustomerEntering = false;
        private float customerLeaveTimer = 0f;
        private float customerXOffset = 0f;
        private Rectangle skipCustomerButton;

        public CraftGreyboxSystem(Texture2D pixel, SpriteFont font, InventorySystem inventory, ContentManager content)
        {
            this.pixel = pixel;
            this.font = font;
            this.inventory = inventory;
            
            texLavender = content.Load<Texture2D>("levander");
            texBlueLotus = content.Load<Texture2D>("blue_lotus");
            texAntiCurse = content.Load<Texture2D>("Anti_curse");
            texSage = content.Load<Texture2D>("sage");
            texLoveRose = content.Load<Texture2D>("loverose");
            texRedPoppy = content.Load<Texture2D>("redpoppy");
            texMarigold = content.Load<Texture2D>("marigold");
            
            texEmptyGlass = content.Load<Texture2D>("emptyglass");
            texPotion = content.Load<Texture2D>("potion");
            texPanjur = content.Load<Texture2D>("Panjur_5");
            texPanjurip = content.Load<Texture2D>("Panjur_ip_6");

            string[] chars = new string[] { "a", "b", "c", "d", "e", "f", "g" ,"h"};
            
            foreach(var c in chars)
            {
                texCustBase[c] = content.Load<Texture2D>(c);
                texCustHappy[c] = content.Load<Texture2D>(c + "_happy");
                texCustAngry[c] = content.Load<Texture2D>(c + "_angry");
            }

            panjurPosition = new Vector2(0, panjurClosedY);
            panjurRopePosition = new Vector2(0, ropeHiddenY);

            panjurButton = new Rectangle(1920 / 2 - 125, 640, 250, 250);
            panjurRopeButton = new Rectangle(1920 / 2 - 80, 520, 160, 220);
            
            skipCustomerButton = new Rectangle(1400, 100, 230, 55);

            texMortar = content.Load<Texture2D>("mortar_0");
            texMortar1 = content.Load<Texture2D>("mortar_1");
            texMortarDust = content.Load<Texture2D>("Mortar_Dust_0");
            texMortarDust1 = content.Load<Texture2D>("Mortar_Dust_1");
            texGrinder = content.Load<Texture2D>("Mortar_Ball");
            texTrash = content.Load<Texture2D>("trash");
            texTable = content.Load<Texture2D>("table");
            texWall = content.Load<Texture2D>("WallPaper_0");
            texBooktab = content.Load<Texture2D>("Book_Tab");
            texLight = content.Load<Texture2D>("Light");
            
            binBox = new Rectangle(1650, 800, 120, 120);
            
            int mortarWidth = (int)(texMortar.Width * mortarScale);
            int mortarHeight = (int)(texMortar.Height * mortarScale);

            Rectangle mortarRect = new Rectangle(
                350,
                680,
                mortarWidth,
                mortarHeight
            );

            mortar = new MortarBox(mortarRect);

            int grinderWidth = (int)(texGrinder.Width * grinderScale);
            int grinderHeight = (int)(texGrinder.Height * grinderScale);

            mortar.Grinder = new Rectangle(
                mortar.Bounds.Right + 25,
                mortar.Bounds.Y - 15,
                grinderWidth,
                grinderHeight
            );

            grinderCurrentRect = mortar.Grinder;

            mortarGrindArea = new Rectangle(
                mortar.Bounds.X + (int)(mortar.Bounds.Width * 0.22f),
                mortar.Bounds.Y + (int)(mortar.Bounds.Height * 0.08f),
                (int)(mortar.Bounds.Width * 0.56f),
                (int)(mortar.Bounds.Height * 0.72f)
            );

            waterBox = new Rectangle(1100, 410, 90, 90); 

            recipeBookButton = new Rectangle(1650, 30, 230, 55);
            recipeBookPanel = new Rectangle(260, 50, 1400, 920);
            closeRecipeBookButton = new Rectangle(1580, 70, 50, 45);

            shopButton = new Rectangle(1400, 30, 230, 55); 
            shopPanel = new Rectangle(460, 240, 1000, 400); 
            closeShopButton = new Rectangle(1400, 260, 40, 40); 

            CreateJarsFromInventory();
            CreateGlassesFromInventory();
            
            RefreshFromInventory();
        }

        public void Update(GameTime gameTime)
        {
            oldMouse = mouse;
            mouse = Mouse.GetState();

            Point mp = mouse.Position;
            HandlePanjurButton(gameTime, mp);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (itemsSlidingOut)
            {
                slideOffset += 1500f * dt;
                
                if (slideOffset > 2000f)
                {
                    slideOffset = 2000f;
                }
            }

            if (currentCustomer != null && currentEmotion != CustomerEmotion.Silhouette && slideOffset >= 2000f)
            {
                // DÜZENLEME: Skip Butonu artık kusursuz çalışıyor!
                if (LeftPressed() && skipCustomerButton.Contains(mp) && !isCustomerLeaving && !isCustomerEntering)
                {
                    currentEmotion = CustomerEmotion.Angry;
                    isCustomerLeaving = true;
                }

                if (isCustomerLeaving)
                {
                    customerLeaveTimer += dt;
                    if (customerLeaveTimer > 1.2f) 
                    {
                        customerXOffset += 2000f * dt; 
                        
                        if (customerXOffset > 1500f)
                        {
                            currentCustomer = new Customers();
                            string[] chars = new string[] { "a", "b", "c", "d", "e", "f", "g" ,"h"};
                            currentCharacter = chars[random.Next(chars.Length)];
                            currentEmotion = CustomerEmotion.Neutral;
                            
                            haggleAttempts = 0;
                            isCustomerLeaving = false;
                            isCustomerEntering = true;
                            
                            customerXOffset = -1500f; 
                            customerLeaveTimer = 0f;
                        }
                    }
                }
                else if (isCustomerEntering)
                {
                    customerXOffset += 2000f * dt; 
                    if (customerXOffset >= 0f)
                    {
                        customerXOffset = 0f;
                        isCustomerEntering = false;
                    }
                }
            }

            if (recipeBookOpen)
            {
                if (LeftPressed() && closeRecipeBookButton.Contains(mp))
                {
                    recipeBookOpen = false;
                }
                return; 
            }

            if (shopOpen)
            {
                if (LeftPressed() && closeShopButton.Contains(mp))
                {
                    shopOpen = false;
                }
                
                HandleShopClicks(mp);
                return; 
            }

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
            
            // DÜZENLEME: Sürükleme mekaniği kontrolü sağlama alındı
            if (slideOffset == 0f && !itemsSlidingOut)
            {
                HandleJarDrag(mp);
                HandleWater(mp);
                HandleGrinderDrag(gameTime, mp);
                HandleMortarDrag(mp);
            }
            else if (slideOffset >= 2000f)
            {
                HandleGlassDrag(mp);
            }
        }

        private void CreateJarsFromInventory()
        {
            jars.Clear();
            int index = 0;

            foreach (var herb in inventory.CollectedHerbs)
            {
                if (herb.Value <= 0)
                {
                    continue;
                }
               
                Rectangle rect = new Rectangle(
                    150 + (index % 7) * 150, 
                    100,                     
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

        private Rectangle GetPanjurRopeButton()
        {
            return new Rectangle(
                (int)panjurRopePosition.X + ropeButtonOffsetX,
                (int)panjurRopePosition.Y + ropeButtonOffsetY,
                ropeButtonWidth,
                ropeButtonHeight
            );
        }

        private void CreateGlassesFromInventory()
        {
            glasses.Clear();

            for (int i = 0; i < inventory.EmptyBottleCount; i++)
            {
                glasses.Add(new PotionGlassBox(NewGlassPosition(i)));
            }
        }

        private void HandlePanjurButton(GameTime gameTime, Point mp)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (panjurButtonVisible && LeftPressed() && panjurButton.Contains(mp))
            {
                panjurOpening = true;
                panjurButtonVisible = false;
                
                itemsSlidingOut = true;

                currentEmotion = CustomerEmotion.Neutral;
            }

            if (panjurOpening)
            {
                panjurPosition.Y -= panjurSpeed * dt;

                if (panjurPosition.Y <= panjurOpenY)
                {
                    panjurPosition.Y = panjurOpenY;
                    panjurOpening = false;

                    ropeVisible = true;
                    ropeComingDown = true;
                    ropeButtonActive = false;
                }
            }

            if (ropeComingDown)
            {
                panjurRopePosition.Y += ropeSpeed * dt;

                if (panjurRopePosition.Y >= ropeDownY)
                {
                    panjurRopePosition.Y = ropeDownY;
                    ropeComingDown = false;

                    ropeButtonActive = true;
                }
            }

            if (ropeButtonActive && LeftPressed() && GetPanjurRopeButton().Contains(mp))
            {
                ropeButtonActive = false;
                ropeGoingUp = true;
                panjurClosing = true;
                
                // DÜZENLEME: Kapanırken itemsSlidingIn aktif ETMİYORUZ, arkada gizli kalacaklar.
            }

            if (ropeGoingUp)
            {
                panjurRopePosition.Y -= ropeSpeed * dt;

                if (panjurRopePosition.Y <= ropeHiddenY)
                {
                    panjurRopePosition.Y = ropeHiddenY;
                    ropeGoingUp = false;
                    ropeVisible = false;
                }
            }

            if (panjurClosing)
            {
                panjurPosition.Y += panjurSpeed * dt;

                if (panjurPosition.Y >= panjurClosedY)
                {
                    panjurPosition.Y = panjurClosedY;
                    panjurClosing = false;
                    panjurButtonVisible = true;
                    
                    currentCustomer = null;
                    OnShopFinished?.Invoke();
                }
            }
        }

        private Rectangle NewGlassPosition(int index)
        {
            float scale = 0.1f; 

            int width = (int)(texEmptyGlass.Width * scale);
            int height = (int)(texEmptyGlass.Height * scale);

            int x = 750 + (index % 6) * (width + 20);
            int y = 750 + (index / 6) * (height + 20);

            return new Rectangle(x, y, width, height);
        }

        public void RefreshFromInventory()
        {
            CreateJarsFromInventory();
            CreateGlassesFromInventory();

            currentCustomer = new Customers();
            string[] chars = new string[] { "a", "b", "c", "d", "e", "f", "g" };
            currentCharacter = chars[random.Next(chars.Length)];
            currentEmotion = CustomerEmotion.Silhouette;

            slideOffset = 0f;
            itemsSlidingOut = false;
            panjurButtonVisible = true;
            ropeVisible = false;
            ropeButtonActive = false;
            panjurOpening = false;
            panjurClosing = false;
            panjurPosition.Y = panjurClosedY;
            panjurRopePosition.Y = ropeHiddenY;
            
            haggleAttempts = 0;
            isCustomerLeaving = false;
            isCustomerEntering = false;
            customerLeaveTimer = 0f;
            customerXOffset = 0f;
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
            {
                return;
            }

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
            {
                return;
            }

            if (!waterBox.Contains(mp)) 
            {
                return;
            }

            if (!mortar.HasBottom || !mortar.HasTop) 
            {
                return;
            }

            hasWater = true;
        }

        private void HandleGrinderDrag(GameTime gameTime, Point mp)
        {
            if (LeftPressed() && grinderCurrentRect.Contains(mp))
            {
                draggingGrinder = true;

                grinderDragOffset = new Vector2(
                    mp.X - grinderCurrentRect.X,
                    mp.Y - grinderCurrentRect.Y
                );
            }

            if (draggingGrinder && mouse.LeftButton == ButtonState.Pressed)
            {
                Rectangle desiredRect = new Rectangle(
                    (int)(mp.X - grinderDragOffset.X),
                    (int)(mp.Y - grinderDragOffset.Y),
                    grinderCurrentRect.Width,
                    grinderCurrentRect.Height
                );

                desiredRect = ApplyMortarSideWallCollision(desiredRect);

                grinderCurrentRect = desiredRect;

                TryGrindWithEnterExit();
            }

            if (draggingGrinder && LeftReleased())
            {
                draggingGrinder = false;
                grinderWasInsideMortar = false;
            }
        }

        private Rectangle ApplyMortarSideWallCollision(Rectangle desiredRect)
        {
            Rectangle outerCollider = mortar.Bounds;

            int leftWallThickness = 55;
            int rightWallThickness = 55;

            int bottomWallThickness = 35;

            if (!desiredRect.Intersects(outerCollider))
            {
                return desiredRect;
            }

            int innerLeft = outerCollider.Left + leftWallThickness;
            int innerRight = outerCollider.Right - rightWallThickness;

            int innerBottom = outerCollider.Bottom - bottomWallThickness;

            int clampedX = (int)MathHelper.Clamp(
                desiredRect.X,
                innerLeft,
                innerRight - desiredRect.Width
            );

            int clampedY = desiredRect.Y;

            if (desiredRect.Bottom > innerBottom)
            {
                clampedY = innerBottom - desiredRect.Height;
            }

            return new Rectangle(
                clampedX,
                clampedY,
                desiredRect.Width,
                desiredRect.Height
            );
        }

        private void TryGrindWithEnterExit()
        {
            if (!mortar.HasBottom || !mortar.HasTop || !hasWater)
            {
                return;
            }

            if (hasMixedPotion)
            {
                return;
            }

            bool grinderInsideMortar = grinderCurrentRect.Intersects(mortarGrindArea);
            
            if (grinderInsideMortar && !grinderWasInsideMortar)
            {
                float grindAmount = 35f + inventory.MortarLevel * 15f;
                grindProgress += grindAmount;

                if (grindProgress >= grindNeeded)
                {
                    FinishGrinding();
                }
            }

            grinderWasInsideMortar = grinderInsideMortar;
        }

        private void FinishGrinding()
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
        
        private void HandleGlassDrag(Point mp)
        {
            if (currentCustomer == null || currentEmotion == CustomerEmotion.Silhouette || isCustomerLeaving || isCustomerEntering) 
            {
                return;
            }

            if (LeftPressed())
            {
                foreach (var glass in glasses)
                {
                    if (glass.IsFilled && glass.Bounds.Contains(mp))
                    {
                        draggedGlass = glass;
                        break;
                    }
                }
            }

            if (draggedGlass != null && mouse.LeftButton == ButtonState.Pressed)
            {
                draggedGlass.DragPosition = new Vector2(mp.X - 50, mp.Y - 50);
            }

            if (draggedGlass != null && LeftReleased())
            {
                Rectangle dragRect = new Rectangle(
                    (int)draggedGlass.DragPosition.X, 
                    (int)draggedGlass.DragPosition.Y, 
                    draggedGlass.Bounds.Width, 
                    draggedGlass.Bounds.Height
                );
                
                Rectangle sellZone = new Rectangle(customerRect.X, customerRect.Y, customerRect.Width, 400);

                if (dragRect.Intersects(sellZone))
                {
                    if (OnSellAttempt != null)
                    {
                        OnSellAttempt(draggedGlass);
                    }
                    else
                    {
                        ResolveSale(draggedGlass, 25);
                    }
                }
                
                draggedGlass.DragPosition = Vector2.Zero;
                draggedGlass = null;
            }
        }

        public void ResolveSale(PotionGlassBox glass, int price)
        {
            if (isCustomerLeaving || isCustomerEntering) return;

            if (price <= currentCustomer.MaxPrice && glass.PotionName == currentCustomer.WantedPotion)
            {
                currentEmotion = CustomerEmotion.Happy;
                inventory.AddMoney(price);
                glass.IsFilled = false;
                
                isCustomerLeaving = true; 
            }
            else
            {
                currentEmotion = CustomerEmotion.Angry;
                haggleAttempts++; 
                
                if (haggleAttempts >= 3)
                {
                    isCustomerLeaving = true; 
                }
            }
        }

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

        private Texture2D GetJarTexture(string herbName)
        {
            switch (herbName)
            {
                case "Lavender": 
                    return texLavender;
                case "Blue Lotus": 
                    return texBlueLotus;
                case "Anti-Curse Clover": 
                    return texAntiCurse;
                case "Sage": 
                    return texSage;
                case "Love Rose": 
                    return texLoveRose;
                case "Red Poppy": 
                    return texRedPoppy;
                case "Marigold": 
                    return texMarigold;
                default: 
                    return texLavender;
            }
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
            {
                return "Sleep Potion";
            }
            if (key1 == "Love Rose+Lavender" || key2 == "Love Rose+Lavender")
            {
                return "Love Potion";
            }
            if (key1 == "Anti-Curse Clover+Sage" || key2 == "Anti-Curse Clover+Sage")
            {
                return "Anti-Curse Potion";
            }
            if (key1 == "Sage+Blue Lotus" || key2 == "Sage+Blue Lotus")
            {
                return "Memory Potion";
            }
            if (key1 == "Red Poppy+Marigold" || key2 == "Red Poppy+Marigold")
            {
                return "Pain Relief Potion";
            }
            if (key1 == "Love Rose+Sage" || key2 == "Love Rose+Sage")
            {
                return "Persuasion Potion";
            }
            if (key1 == "Lavender+Anti-Curse Clover" || key2 == "Lavender+Anti-Curse Clover")
            {
                return "Purification Potion";
            }
            if (key1 == "Lavender+Sage" || key2 == "Lavender+Sage")
            {
                return "Relaxation Potion";
            }
            if (key1 == "Lavender+Red Poppy" || key2 == "Lavender+Red Poppy")
            {
                return "Soothing Potion";
            }
            if (key1 == "Blue Lotus+Love Rose" || key2 == "Blue Lotus+Love Rose")
            {
                return "Mystic Romance Potion";
            }
            if (key1 == "Blue Lotus+Anti-Curse Clover" || key2 == "Blue Lotus+Anti-Curse Clover")
            {
                return "Holy Water Potion";
            }
            if (key1 == "Love Rose+Anti-Curse Clover" || key2 == "Love Rose+Anti-Curse Clover")
            {
                return "Heart Protection Potion";
            }
            if (key1 == "Love Rose+Red Poppy" || key2 == "Love Rose+Red Poppy")
            {
                return "Passion Potion";
            }
            if (key1 == "Anti-Curse Clover+Red Poppy" || key2 == "Anti-Curse Clover+Red Poppy")
            {
                return "Vitality Potion";
            }
            if (key1 == "Sage+Red Poppy" || key2 == "Sage+Red Poppy")
            {
                return "Focus Potion";
            }
            if (key1 == "Sage+Marigold" || key2 == "Sage+Marigold")
            {
                return "Enlightenment Potion";
            }
            if (key1 == "Lavender+Lavender")
            {
                return "Calm Potion";
            }
            if (key1 == "Blue Lotus+Blue Lotus")
            {
                return "Clarity Potion";
            }
            if (key1 == "Love Rose+Love Rose")
            {
                return "Charm Potion";
            }
            if (key1 == "Anti-Curse Clover+Anti-Curse Clover")
            {
                return "Ward Potion";
            }
            if (key1 == "Sage+Sage")
            {
                return "Wisdom Potion";
            }
            if (key1 == "Red Poppy+Red Poppy")
            {
                return "Rage Potion";
            }
            if (key1 == "Marigold+Marigold")
            {
                return "Bright Potion";
            }

            return "Unknown Potion";
        }

        private Color Mix(Color a, Color b)
        {
            return new Color((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texWall, Vector2.Zero, Color.White);

            if (currentCustomer != null)
            {
                Texture2D cTex = texCustBase[currentCharacter];
                
                if (currentEmotion == CustomerEmotion.Happy) 
                {
                    cTex = texCustHappy[currentCharacter];
                }
                else if (currentEmotion == CustomerEmotion.Angry) 
                {
                    cTex = texCustAngry[currentCharacter];
                }
                
                float lerpAmount = (panjurPosition.Y - panjurClosedY) / (panjurOpenY - panjurClosedY);
                lerpAmount = MathHelper.Clamp(lerpAmount, 0f, 1f);
                
                // DÜZENLEME: Karakter saydamlaşmayacak, sadece kararacak (Alpha değeri 1f'de sabit!)
                Color cCol = new Color(lerpAmount, lerpAmount, lerpAmount, 1f);

                Rectangle currentCustRect = new Rectangle(
                    customerRect.X + (int)customerXOffset, 
                    customerRect.Y, 
                    customerRect.Width, 
                    customerRect.Height
                );

                spriteBatch.Draw(cTex, currentCustRect, cCol);

                if (currentEmotion != CustomerEmotion.Silhouette && lerpAmount > 0.05f)
                {
                    // DÜZENLEME: Kullanıcı iksirleri rahat test etsin diye WantedPotion ipucu eklendi!
                    string text = currentCustomer.CurrentDialogue + $"\n(Needs: {currentCustomer.WantedPotion})";
                    
                    if (currentEmotion == CustomerEmotion.Happy) 
                    {
                        text = "Thank you! It's perfect.";
                    }
                    else if (currentEmotion == CustomerEmotion.Angry) 
                    {
                        if (isCustomerLeaving) text = "I've had enough. I'm leaving!";
                        else if (haggleAttempts == 1) text = "Is this a joke? That's not what I asked for!";
                        else if (haggleAttempts == 2) text = "Are you trying to scam me? One last chance!";
                    }
                    
                    Color textCol = Color.White * lerpAmount;
                    spriteBatch.DrawString(font, text, new Vector2(currentCustRect.Right - 50, currentCustRect.Top + 50), textCol);
                }
            }

            spriteBatch.Draw(texLight, Vector2.Zero, Color.White);
            spriteBatch.Draw(texPanjur, panjurPosition, Color.White);
            spriteBatch.Draw(texPanjurip, panjurRopePosition, Color.White);
            
            if (panjurButtonVisible)
            {
                spriteBatch.Draw(pixel, panjurButton, Color.Red * 0.0f);
            }

            if (ropeButtonActive)
            {
                spriteBatch.Draw(pixel, GetPanjurRopeButton(), Color.Blue * 0.0f);
            }
            
            spriteBatch.Draw(texBooktab, Vector2.Zero, Color.White);
            spriteBatch.Draw(texTable, Vector2.Zero, Color.White);
            
            DrawMoney(spriteBatch);     
            DrawJars(spriteBatch);
            DrawMortar(spriteBatch);
            DrawWater(spriteBatch);
            DrawGlasses(spriteBatch);
            DrawCraftedPotions(spriteBatch);
            DrawBin(spriteBatch);
            
            DrawBox(spriteBatch, recipeBookButton, Color.DarkSlateGray);
            spriteBatch.DrawString(font, "Recipe Book", new Vector2(recipeBookButton.X + 35, recipeBookButton.Y + 15), Color.White);

            DrawBox(spriteBatch, shopButton, Color.DarkOliveGreen);
            spriteBatch.DrawString(font, "Open Shop", new Vector2(shopButton.X + 45, shopButton.Y + 15), Color.White);

            if (currentCustomer != null && currentEmotion != CustomerEmotion.Silhouette && slideOffset >= 2000f)
            {
                DrawBox(spriteBatch, skipCustomerButton, Color.DarkRed);
                spriteBatch.DrawString(font, "Next Customer", new Vector2(skipCustomerButton.X + 25, skipCustomerButton.Y + 15), Color.White);
            }

            if (draggingMortar && hasMixedPotion)
            {
                spriteBatch.Draw(pixel, new Rectangle(mouse.X - 30, mouse.Y - 30, 60, 60), mixedColor);
                spriteBatch.DrawString(font, craftedPotionName, new Vector2(mouse.X + 35, mouse.Y - 10), Color.White);
            }

            if (draggedGlass != null)
            {
                Rectangle r = new Rectangle(
                    (int)draggedGlass.DragPosition.X, 
                    (int)draggedGlass.DragPosition.Y, 
                    draggedGlass.Bounds.Width, 
                    draggedGlass.Bounds.Height
                );
                
                spriteBatch.Draw(texEmptyGlass, r, Color.White);
                spriteBatch.Draw(texPotion, r, draggedGlass.FillColor);
            }

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
            
            string[] col1Recipes = new string[] 
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

            string[] col2Recipes = new string[] 
            {
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
        public Vector2 DragPosition; 

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