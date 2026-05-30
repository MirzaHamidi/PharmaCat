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

        private Texture2D pixel;
        private SpriteFont font;
        private InventorySystem inventory;

        private Texture2D texLavender, texBlueLotus, texAntiCurse, texSage;
        private Texture2D texLoveRose, texRedPoppy, texMarigold;
        private Texture2D texEmptyGlass,texPen, texPotion, texMortar1, texMortar, texGrinder, texTrash, texTable, texWall, texLight, texPanjur, texBooktab, texCodex ,texMortarDust, texMortarDust1, texPanjurip;

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
        private Rectangle currentDialogueBubbleRect;
        private Rectangle targetDialogueBubbleRect;

        private bool dialogueBubbleInitialized = false;
        private const int DialogueBubbleMinWidth = 320;
        private const int DialogueBubbleMaxWidth = 760;
        private const int DialogueBubbleMinHeight = 110;
        private const int DialogueBubbleMaxHeight = 300;

        private const int DialogueBubblePaddingX = 34;
        private const int DialogueBubblePaddingY = 30;
        private Vector2 panjurPosition;
        private Vector2 panjurRopePosition;


        private bool recipeBookAnimatingOpen;
        private bool recipeBookAnimatingClose;

        private Vector2 bookTabPosition;
        private Vector2 codexPosition;

        private Vector2 bookTabClosedPosition = Vector2.Zero;
        private Vector2 bookTabOpenPosition = new Vector2(0, -220);

        private Vector2 codexClosedPosition = new Vector2(0, -1100);
        private Vector2 codexOpenPosition = Vector2.Zero;

        private float bookAnimationSpeed = 1800f;

        private bool panjurOpening;
        private bool panjurClosing;

        private bool ropeComingDown;
        private bool ropeGoingUp;

        private bool panjurButtonVisible = true;
        private bool ropeButtonActive = false;
        private bool ropeVisible = false;

        private float panjurSpeed = 250f;
        private float ropeSpeed = 195f;

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

        private bool isCustomerLeaving = false;
        private bool isCustomerEntering = false;
        private float customerLeaveTimer = 0f;
        private float customerXOffset = 0f;
        private Rectangle skipCustomerButton;

        private readonly TypewriterDialogue dialogue = new TypewriterDialogue();
        private readonly string[] customerCharacters = { "a", "b", "c", "d", "e", "f", "g", "h" };
        private bool introDialogueStarted = false;

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

            foreach(var c in customerCharacters)
            {
                texCustBase[c] = content.Load<Texture2D>(c);
                texCustHappy[c] = content.Load<Texture2D>(c + "_happy");
                texCustAngry[c] = content.Load<Texture2D>(c + "_angry");
            }

            panjurPosition = new Vector2(0, panjurClosedY);
            panjurRopePosition = new Vector2(0, ropeHiddenY);

            panjurButton = new Rectangle(1920 / 2 - 125, 640, 250, 250);
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
            texPen = content.Load<Texture2D>("Pen_prop");
            

            bookTabPosition = bookTabClosedPosition;
            codexPosition = codexClosedPosition;


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

            recipeBookButton = new Rectangle(1790, 90, 45, 105);
            recipeBookPanel = new Rectangle(260, 50, 1400, 920);
            closeRecipeBookButton = new Rectangle(1580, 70, 50, 45);

            shopButton = new Rectangle(1400, 30, 230, 55); 
            shopPanel = new Rectangle(460, 240, 1000, 400); 
            closeShopButton = new Rectangle(1400, 260, 40, 40); 

            CreateJarsFromInventory();
            CreateGlassesFromInventory();
            
            RefreshFromInventory();
            texCodex = content.Load<Texture2D>("Codex_open");
        }

        public void Update(GameTime gameTime)
        {
            oldMouse = mouse;
            mouse = Mouse.GetState();

            Point mp = mouse.Position;
            HandlePanjurButton(gameTime, mp);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateRecipeBookAnimation(dt);

            dialogue.Update(gameTime);

            if (dialogue.FinishedWaiting)
            {
                dialogue.Clear();
                customerLeaveTimer = 0f;
                isCustomerLeaving = true;
            }

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
                if (LeftPressed() && skipCustomerButton.Contains(mp) && !isCustomerLeaving && !isCustomerEntering && !dialogue.IsTyping)
                {
                    currentEmotion = CustomerEmotion.Angry;
                    StartResultDialogue("How rude! I am leaving this cursed shop!", false);
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
                            currentCharacter = customerCharacters[random.Next(customerCharacters.Length)];
                            currentEmotion = CustomerEmotion.Neutral;
                            
                            isCustomerLeaving = false;
                            isCustomerEntering = true;
                            introDialogueStarted = false;
                            dialogue.Clear();
                            
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
                        StartIntroDialogue();
                    }
                }
            }

            if (!introDialogueStarted && currentCustomer != null && currentEmotion != CustomerEmotion.Silhouette && !isCustomerEntering && !isCustomerLeaving && slideOffset >= 2000f && panjurPosition.Y <= panjurOpenY + 1f)
            {
                StartIntroDialogue();
            }

            if (recipeBookOpen || recipeBookAnimatingOpen || recipeBookAnimatingClose)
{
    if (recipeBookOpen && LeftPressed())
    {
        Rectangle codexRect = GetCodexRect();

        if (!codexRect.Contains(mp))
        {
            recipeBookAnimatingClose = true;
            recipeBookOpen = false;
        }
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

            if (LeftPressed() && recipeBookButton.Contains(mp) && !recipeBookOpen && !recipeBookAnimatingOpen && !recipeBookAnimatingClose)
            {
            recipeBookAnimatingOpen = true;
            return;
            }

            if (LeftPressed() && shopButton.Contains(mp))
            {
                shopOpen = true;
                return;
            }
            
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
        private void UpdateRecipeBookAnimation(float dt)
        {
        if (recipeBookAnimatingOpen)
        {
        bookTabPosition = MoveTowards(bookTabPosition, bookTabOpenPosition, bookAnimationSpeed * dt);
        codexPosition = MoveTowards(codexPosition, codexOpenPosition, bookAnimationSpeed * dt);

        if (bookTabPosition == bookTabOpenPosition && codexPosition == codexOpenPosition)
        {
            recipeBookAnimatingOpen = false;
            recipeBookOpen = true;
        }
        }

        if (recipeBookAnimatingClose)
        {
        bookTabPosition = MoveTowards(bookTabPosition, bookTabClosedPosition, bookAnimationSpeed * dt);
        codexPosition = MoveTowards(codexPosition, codexClosedPosition, bookAnimationSpeed * dt);

        if (bookTabPosition == bookTabClosedPosition && codexPosition == codexClosedPosition)
        {
            recipeBookAnimatingClose = false;
            recipeBookOpen = false;
        }
        }
        }

        private Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistance)
        {
        Vector2 direction = target - current;
        float distance = direction.Length();

        if (distance <= maxDistance || distance == 0f)
        return target;

        direction.Normalize();
        return current + direction * maxDistance;
        }


            private Rectangle GetCodexRect()
{
    return new Rectangle(
        (int)codexPosition.X,
        (int)codexPosition.Y,
        texCodex.Width / 2,
        texCodex.Height/2
    );
}


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texWall, Vector2.Zero, Color.White);

            if (currentCustomer != null)
            {
                Texture2D cTex = GetCurrentCustomerTexture();
                
                float lerpAmount = (panjurPosition.Y - panjurClosedY) / (panjurOpenY - panjurClosedY);
                lerpAmount = MathHelper.Clamp(lerpAmount, 0f, 1f);
                
                Color cCol = new Color(lerpAmount, lerpAmount, lerpAmount, 1f);

                Rectangle currentCustRect = new Rectangle(
                    customerRect.X + (int)customerXOffset, 
                    customerRect.Y, 
                    customerRect.Width, 
                    customerRect.Height
                );

                spriteBatch.Draw(cTex, currentCustRect, cCol);

                if (currentEmotion != CustomerEmotion.Silhouette &&
    lerpAmount > 0.05f &&
    !string.IsNullOrEmpty(dialogue.VisibleText))
{
    Color textCol = Color.White * lerpAmount;

    DrawDialogueBubble(spriteBatch, currentCustRect, textCol);

    float textMaxWidth = DialogueBubbleMaxWidth - DialogueBubblePaddingX * 2;
    string text = WrapText(dialogue.VisibleText, textMaxWidth);

    spriteBatch.DrawString(
        font,
        text,
        new Vector2(
            currentDialogueBubbleRect.X + DialogueBubblePaddingX,
            currentDialogueBubbleRect.Y + DialogueBubblePaddingY
        ),
        textCol
    );
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
            
            spriteBatch.Draw(texBooktab, bookTabPosition, Color.White);

            if (recipeBookOpen || recipeBookAnimatingOpen || recipeBookAnimatingClose)
            {
            
            }
            spriteBatch.Draw(texTable, Vector2.Zero, Color.White);
            
            DrawMoney(spriteBatch);     
            DrawJars(spriteBatch);
            DrawMortar(spriteBatch);
            DrawWater(spriteBatch);
            DrawGlasses(spriteBatch);
            DrawCraftedPotions(spriteBatch);
            DrawBin(spriteBatch);
            DrawBox(spriteBatch, shopButton, Color.DarkOliveGreen);
            spriteBatch.DrawString(font, "Open Shop", new Vector2(shopButton.X + 45, shopButton.Y + 15), Color.White);

            if (currentCustomer != null && currentEmotion != CustomerEmotion.Silhouette && slideOffset >= 2000f && !dialogue.IsTyping && !isCustomerLeaving && !isCustomerEntering)
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
            spriteBatch.Draw(texPen, Vector2.Zero, Color.White);
            if (shopOpen)
            {
                DrawShop(spriteBatch);
            }
            spriteBatch.Draw(texCodex, codexPosition, Color.White); 
            if (recipeBookOpen)
            {
            //DrawRecipeBookContent(spriteBatch);
            }     

                 
        }
    }

}
