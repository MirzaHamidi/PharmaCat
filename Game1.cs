using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PharmaCat.Scripts;
using Myra;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Linq;

namespace PharmaCat;

public class Game1 : Game
{
    private enum GameState // these are the scenes of the game, we will switch between them
    {
        MainMenu,
        Jungle,
        Shop,
        Crafting,
        Paused,
        GameOver
    }
    private ShopSystem _shopSystem;
    private NarratorSystem _narratorSystem;
    private Customers currentCustomer;
    private TextButton _serveButton;
    private TextButton _nextCustomerButton;
    private Label _customerDialogueLabel;
    private TextBox _priceBox;
    private ComboBox _potionBox;
    private Label _resultLabel;
    private Desktop _menuDesktop;
    private Desktop _shopDesktop;
    private Desktop _craftingDesktop;
    private TextButton _startButton;
    private TextButton _gotoshop;
    private List<TiledIsoEntity> trees = new List<TiledIsoEntity>();
    private List<TiledIsoEntity> bushes = new List<TiledIsoEntity>();

    private Texture2D treeTexture;
    private Texture2D bushTexture;

    private int herbCount = 0;  
    private bool cameraInitialized = false;
    public Vector2 targetPosition { get; private set; }
    private Vector2 cameraOffset = new Vector2(0, 0);
    private SpriteFont font;
    private GameState _gameState = GameState.MainMenu; // start at main menu
    private Texture2D jungleMapTexture; // this is the test bg picture, we will replace it with procedural generated map later
    private Texture2D table;
    private Player player; // player
    private Camera2D camera; // camera for jungle scene
    private float targetZoom = 1f; // camera zoom
    private Vector2 spritePosition; 
    private GraphicsDeviceManager _graphics; // graphics manager
    private SpriteBatch _spriteBatch; // for drawing sprites
    private InputState _input; // input class call
    private float jg_Counter = 150f; // this is the counter for jungle scene, we will use it for day/night cycle.
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this); 
        Content.RootDirectory = "Content"; // content folder
        IsMouseVisible = true; // show mouse cursor
        _graphics.PreferredBackBufferWidth = 1920; // set resolution to 1080p
        _graphics.PreferredBackBufferHeight = 1080; // set resolution to 1080p
        _graphics.IsFullScreen = false; // start in windowed mode
        _graphics.ApplyChanges(); // apply graphics settings
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        camera = new Camera2D(); // initialize camera
        _input = new InputState(); // initialize input
        base.Initialize(); 
    }

    protected override void LoadContent()
{
    font = Content.Load<SpriteFont>("Font");
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    jungleMapTexture = Content.Load<Texture2D>("mapjungle");
    table = Content.Load<Texture2D>("table");

    treeTexture = Texture2D.FromStream(
    GraphicsDevice,
    System.IO.File.OpenRead("Content/agacmap.PNG"));

    bushTexture = Texture2D.FromStream(
    GraphicsDevice,
    System.IO.File.OpenRead("Content/bitkimap.PNG"));

    Vector2 mapCenter = new Vector2(
    jungleMapTexture.Width / 2f,
    jungleMapTexture.Height / 2f
    );

    player = new Player(Content.Load<Texture2D>("cat"), mapCenter);

    CreateWorldEntities();

    MyraEnvironment.Game = this;

    CreateMainMenu();
    CreateCraftingMenu();
    CreateShopMenu();
}

        

        //Loadcontent is for preparing assets for the game this is the current assets can be used in game
    

    protected override void Update(GameTime gameTime)
    {
    _input.Update(); // update input states

    if (_input.FullScreen()) // toggle fullscreen on F4 key press the bindings are in InputState.cs
    {
        _graphics.IsFullScreen = !_graphics.IsFullScreen;
        _graphics.ApplyChanges();
    }

    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) // exit game on escape key 
    {
        Exit();
    }

    switch (_gameState) // in here we assigning different updates for different scenes so game logic will be separated and easier to manage, we will add more scenes later like crafting and shop
    {
        case GameState.MainMenu:
            UpdateMenu(gameTime);
            break;

        case GameState.Jungle:
            UpdateJungle(gameTime);
            break;

        case GameState.Shop:
            UpdateShop(gameTime);
            break;

        case GameState.Crafting:
            UpdateCrafting(gameTime);
            break;
    }

    base.Update(gameTime);
}
    private void UpdateJungle(GameTime gameTime)
{
    if (jg_Counter > 0)
    {
        jg_Counter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (jg_Counter < 0)
        {
            jg_Counter = 0f;
            _gameState = GameState.Crafting;
        }
    }

    int scrollDelta = _input.MouseScrollDelta();

    if (scrollDelta > 0)
        targetZoom += 0.1f;
    else if (scrollDelta < 0)
        targetZoom -= 0.1f;

    targetZoom = MathHelper.Clamp(targetZoom, 2f, 2.7f);

    camera.Zoom = MathHelper.Lerp(
        camera.Zoom,
        targetZoom,
        8f * (float)gameTime.ElapsedGameTime.TotalSeconds
    );

    if (_input.RightClick())
    {
        Vector2 mouseScreenPos = new Vector2(_input._mouseNow.X, _input._mouseNow.Y);
        Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, GraphicsDevice.Viewport);

        player.SetTargetPosition(mouseWorldPos);
    }

    player.Update(gameTime);

    UpdateEntityAlpha();

    if (_input.KeyPressed(Keys.E))
    {
        foreach (var bush in bushes)
        {
            if (!bush.Collected &&
                Vector2.Distance(player.Position, bush.Position) < 80f)
            {
                bush.Collected = true;
                herbCount++;
                break;
            }
        }
    }

    float smoothSpeed = 3f;
    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

    Vector2 moveDirection = player.targetPosition - player.Position;

    if (!cameraInitialized)
    {
        camera.Position = player.Position;
        cameraInitialized = true;
    }

    if (moveDirection.LengthSquared() > 0.01f)
        moveDirection.Normalize();
    else
        moveDirection = Vector2.Zero;

    float lookAheadScreenPixels = 200f;
    float lookAheadWorldDistance = lookAheadScreenPixels / camera.Zoom;

    Vector2 targetCameraPosition =
        player.Position + moveDirection * lookAheadWorldDistance;

    camera.Position = Vector2.Lerp(
        camera.Position,
        targetCameraPosition,
        smoothSpeed * deltaTime
    );
}

    private void UpdateEntityAlpha()
{
    foreach (var tree in trees)
    {
        bool playerInsideForestZone =
            player.Position.Y < 950f;

        tree.Alpha =
            playerInsideForestZone
            ? 0.45f
            : 1f;
    }

    foreach (var bush in bushes)
    {
        bool playerInsideBushZone =
            player.Position.Y < 900f;

        bush.Alpha =
            playerInsideBushZone
            ? 0.65f
            : 1f;
    }
}
    private void CreateWorldEntities()
{
    trees.Clear();
    bushes.Clear();

    LoadEntityLayerFromCsv("Content/bitki_agac.csv", treeTexture, trees, false);
    LoadEntityLayerFromCsv("Content/bitki_cicek.csv", bushTexture, bushes, true);
}
    private void UpdateMenu(GameTime gameTime) // main menu update logic
    {
        
    }

    private void UpdateShop(GameTime gameTime) // shop update logic
    {
        
    }

    private void UpdateCrafting(GameTime gameTime) // crafting update logic
    {
        
    } 

   protected override void Draw(GameTime gameTime) // in here we assigning different draw calls for different scenes so game rendering will be separated and easier to manage, we will add more scenes later like crafting and shop
{
    switch (_gameState)
    {
        case GameState.MainMenu:
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            DrawMenu();
            _spriteBatch.End();
            break;

        case GameState.Jungle:
        GraphicsDevice.Clear(Color.ForestGreen);

        _spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(GraphicsDevice.Viewport));
        DrawJungle();
        _spriteBatch.End();

        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, $"{Math.Ceiling(jg_Counter)}", new Vector2(1600, 100), Color.White);
        _spriteBatch.DrawString(font, $"Herb: {herbCount}", new Vector2(1600, 150), Color.White);
        _spriteBatch.End();

        break;

        case GameState.Shop:
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            DrawShop();
            _spriteBatch.End();
            break;

        case GameState.Crafting:
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            DrawCrafting();
            _spriteBatch.End();
            break;
    }

    base.Draw(gameTime);
}
    private void DrawMenu() // main menu draw logic
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        if (_menuDesktop != null)
        {
            _menuDesktop.Render();
        }
    }
    private Point WorldToIsoGrid(Vector2 worldPos)
    {
    int tileWidth = 64;
    int tileHeight = 32;

    float halfW = tileWidth / 2f;
    float halfH = tileHeight / 2f;

    int gridX = (int)MathF.Floor((worldPos.X / halfW + worldPos.Y / halfH) / 2f);
    int gridY = (int)MathF.Floor((worldPos.Y / halfH - worldPos.X / halfW) / 2f);

    return new Point(gridX, gridY);
    }
    private void DrawJungle()
{
    _spriteBatch.Draw(jungleMapTexture, Vector2.Zero, Color.White);

    var renderList = new List<object>();

    renderList.Add(player);
    renderList.AddRange(trees);
    renderList.AddRange(bushes);

    foreach (var item in renderList.OrderBy(x =>
    {
        if (x is Player p)
            return p.Position.Y;

        if (x is TiledIsoEntity e)
            return e.Position.Y;

        return 0f;
    }))
    {
        if (item is Player p)
            p.Draw(_spriteBatch);

        if (item is TiledIsoEntity e)
            e.Draw(_spriteBatch);
    }
}
private void LoadEntityLayerFromCsv(
    string csvPath,
    Texture2D texture,
    List<TiledIsoEntity> targetList,
    bool isBush)
{
    string[] lines = System.IO.File.ReadAllLines(csvPath);

    int tileWidth = 100;
    int tileHeight = 60;

    for (int y = 0; y < lines.Length; y++)
    {
        string[] values = lines[y].Split(',');

        for (int x = 0; x < values.Length; x++)
        {
            if (!int.TryParse(values[x], out int gid))
                continue;

            if (gid == 0)
            continue;

            


            if (!IsEntityTile(gid, isBush))
                continue;

            float worldX = x * tileWidth;

            if (y % 2 == 1)
                worldX += tileWidth / 2f;

            float worldY = y * (tileHeight / 2f);

            Vector2 basePosition = new Vector2(
                worldX + tileWidth / 2f,
                worldY + tileHeight
            );

            Rectangle source = new Rectangle(
                0,
                0,
                texture.Width,
                texture.Height
            );

            targetList.Add(
                new TiledIsoEntity(
                    texture,
                    source,
                    basePosition,
                    isBush
                )
            );
        }
    }
}
private bool IsEntityTile(int gid, bool isBush)
{
    if (isBush)
    {
        return gid == 39;
    }
    else
    {
        return gid == 7;
    }
}
    private void DrawShop() // shop draw logic
    {
        
    if (_shopDesktop != null)
    {
        _shopDesktop.Render();
    }
    }
    

    private void DrawCrafting() // crafting draw logic
    {
        _spriteBatch.Draw(table, Vector2.Zero, Color.White); 
        if (_craftingDesktop != null)
        {
            _craftingDesktop.Render();
        }
        // draw crafting table in the center of the screen, we will replace it with actual crafting UI later

    }

    
   private void CreateMainMenu()
    {
    var panel = new Panel();

    _startButton = new TextButton
    {
        Text = "Start Game",
        Width = 220,
        Height = 70,
        Left = 850,
        Top = 500
    };

    _startButton.Click += (s, a) =>
    {
        _gameState = GameState.Jungle;
    };

    panel.Widgets.Add(_startButton);
    _menuDesktop = new Desktop();
    _menuDesktop.Root = panel;
}

private void CreateCraftingMenu()
{
   
    
    var panel = new Panel();

    _gotoshop = new TextButton
    {
        Text = "Go to Shop",
        Width = 220,
        Height = 70,
        Left = 850,
        Top = 500
    };

    _gotoshop.Click += (s, a) =>
    {
        _gameState = GameState.Shop;
    };
    panel.Widgets.Add(_gotoshop);
    _craftingDesktop = new Desktop();
    _craftingDesktop.Root = panel;

    
}

private void CreateShopMenu()
{
    currentCustomer = new Customers();

    var panel = new Panel();

    _customerDialogueLabel = new Label
    {
        Text = currentCustomer.CurrentDialogue,
        Left = 850,
        Top = 100,
        Width = 700,
        Height = 120
    };

    _serveButton = new TextButton
    {
        Text = "Serve Customer",
        Left = 850,
        Top = 250,
        Width = 220,
        Height = 60
    };

    _potionBox = new ComboBox
    {
        Left = 850,
        Top = 340,
        Width = 250,
        Height = 40,
        Visible = false
    };

    _potionBox.Items.Add(new ListItem("Sleep Potion"));
    _potionBox.Items.Add(new ListItem("Memory Potion"));
    _potionBox.Items.Add(new ListItem("Love Potion"));
    _potionBox.Items.Add(new ListItem("Anti-Curse Potion"));

    _priceBox = new TextBox
    {
        Left = 850,
        Top = 400,
        Width = 250,
        Height = 40,
        Text = "10",
        Visible = false
    };

    var sellButton = new TextButton
    {
        Text = "Sell",
        Left = 850,
        Top = 460,
        Width = 220,
        Height = 60,
        Visible = false
    };

    _resultLabel = new Label
    {
        Text = "",
        Left = 850,
        Top = 540,
        Width = 600,
        Height = 80
    };

    _nextCustomerButton = new TextButton
    {
        Text = "Next Customer",
        Left = 850,
        Top = 640,
        Width = 220,
        Height = 60,
        Visible = false
    };

    _serveButton.Click += (s, a) =>
{
    _potionBox.Visible = true;
    _priceBox.Visible = true;
    sellButton.Visible = true;
};

sellButton.Click += (s, a) =>
{
    string potion = _potionBox.SelectedItem.Text;

    int price = int.Parse(_priceBox.Text);

    _resultLabel.Text =
        _narratorSystem.GetSellResultText(
            currentCustomer,
            potion,
            price
        );

    _nextCustomerButton.Visible = true;
};

_nextCustomerButton.Click += (s, a) =>
{
    currentCustomer = new Customers();

    _customerDialogueLabel.Text =
        currentCustomer.CurrentDialogue;

    _resultLabel.Text =
        _narratorSystem.GetWaitingText();

    _priceBox.Text = "10";

    _potionBox.Visible = false;
    _priceBox.Visible = false;

    sellButton.Visible = false;
    _nextCustomerButton.Visible = false;
};

panel.Widgets.Add(_customerDialogueLabel);
panel.Widgets.Add(_serveButton);
panel.Widgets.Add(_potionBox);
panel.Widgets.Add(_priceBox);
panel.Widgets.Add(sellButton);
panel.Widgets.Add(_resultLabel);
panel.Widgets.Add(_nextCustomerButton);

_shopDesktop = new Desktop();
_shopDesktop.Root = panel;
}
}
