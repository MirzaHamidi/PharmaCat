using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PharmaCat.Scripts;
using Myra;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Linq;

using PharmaCat.Scripts.Rendering;
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
    private ShopScene shopScene;
    private MainMenuScene mainMenuScene;
    private JungleScene jungleScene;
    private CraftingScene craftingScene;
    private SpriteFont font;
    private GameState _gameState = GameState.MainMenu; // start at main menu
    private Texture2D table;
    private Texture2D Wall;
    private GraphicsDeviceManager _graphics; // graphics manager
    private SpriteBatch _spriteBatch; // for drawing sprites
    private InputState _input; // input class call
    private InventorySystem inventory; // inventory system call
    
    private Random random = new Random();
    
    private Texture2D pixel;
    
    private CraftGreyboxSystem craftGreyboxSystem;
    
    // EKLENDİ: Transition Manager
    private TransitionManager transitionManager;
    private Texture2D kepenkTexture;

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
        _input = new InputState();
        inventory = new InventorySystem();
        MyraEnvironment.Game = this;
        base.Initialize();
    }

    protected override void LoadContent()
{
    font = Content.Load<SpriteFont>("Font");
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    pixel = new Texture2D(GraphicsDevice, 1, 1);
    pixel.SetData(new[] { Color.White });

    table = Content.Load<Texture2D>("table");
    Wall = Content.Load<Texture2D>("WallPaper_0");
    craftingScene = new CraftingScene();
    craftingScene.Load(table);

    craftGreyboxSystem = new CraftGreyboxSystem(pixel, font, inventory, Content);

    kepenkTexture = Content.Load<Texture2D>("kepenk");
    transitionManager = new TransitionManager(pixel, kepenkTexture);

    jungleScene = new JungleScene();
    jungleScene.Load(Content, GraphicsDevice, inventory);

    shopScene = new ShopScene();
    shopScene.Load(inventory, () =>
    {
        transitionManager.StartTransition(TransitionStyle.Shutter, () =>
        {
            jungleScene.ResetDay();
            craftGreyboxSystem.RefreshFromInventory();
            _gameState = GameState.Jungle;
        });
    });

    mainMenuScene = new MainMenuScene();
    mainMenuScene.Load();
}
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

        // Eğer şu an bir geçiş (transition) animasyonu oynuyorsa sahne mekaniklerini dondurabilirsin
        // veya arkada akmaya devam edebilirler. Biz çalışmasına izin verip, sadece Inputları kısıtlayabiliriz.
        // Şimdilik sorunsuz akması için Update'leri normal bırakıyoruz.

        switch (_gameState) 
        {
            case GameState.MainMenu:
                mainMenuScene.Update(gameTime);

                if (mainMenuScene.StartRequested)
                {
                    mainMenuScene.ResetRequest();
                    // Main Menu -> Jungle Geçişi (Kararma)
                    transitionManager.StartTransition(TransitionStyle.Fade, () => 
                    {
                        _gameState = GameState.Jungle;
                    });
                }
                break;

            case GameState.Jungle:
                jungleScene.Update(gameTime, _input, GraphicsDevice.Viewport);

                if (jungleScene.CraftingRequested)
                {
                    jungleScene.ResetRequest();
                    // DÜZENLEME: Jungle -> Crafting Geçişi (Smooth Kararma)
                    transitionManager.StartTransition(TransitionStyle.Fade, () =>
                    {
                        craftGreyboxSystem.RefreshFromInventory();
                        _gameState = GameState.Crafting;
                    });
                }
                break;

            case GameState.Shop:
                shopScene.Update(gameTime);
                break;

            case GameState.Crafting:
                craftingScene.Update(gameTime);
                craftGreyboxSystem.Update(gameTime);
                if (craftingScene.GoToShopRequested)
                {
                    craftingScene.ResetRequest();
                    // DÜZENLEME: Crafting -> Shop Geçişi (Kepenk)
                    transitionManager.StartTransition(TransitionStyle.Shutter, () =>
                    {
                        _gameState = GameState.Shop;
                    });
                }
                break;
        }

        // EKLENDİ: Transition Manager'ı Güncelle
        transitionManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) 
    {
        switch (_gameState)
        {
            case GameState.MainMenu:
                GraphicsDevice.Clear(Color.CornflowerBlue);
                _spriteBatch.Begin();
                mainMenuScene.Draw();
                _spriteBatch.End();
                break;

            case GameState.Jungle:
                GraphicsDevice.Clear(Color.ForestGreen);
                jungleScene.DrawWorld(_spriteBatch, GraphicsDevice.Viewport);
                jungleScene.DrawUi(_spriteBatch, font);
                break;

            case GameState.Shop:
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                shopScene.Draw();
                _spriteBatch.End();
                break;

            case GameState.Crafting:
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                craftingScene.Draw(_spriteBatch);
                craftGreyboxSystem.Draw(_spriteBatch);
                _spriteBatch.End();
                break;
        }

        // EKLENDİ: Transition animasyonu bütün sahnelerin "en üstünde" çizilmeli
        _spriteBatch.Begin();
        transitionManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}