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
    private enum GameState 
    {
        MainMenu,
        Jungle,
        Crafting, // Shop artık Crafting'in içinde olduğu için buradan kaldırıldı!
        Paused,
        GameOver
    }
    
    private MainMenuScene mainMenuScene;
    private JungleScene jungleScene;
    private CraftingScene craftingScene;
    private SpriteFont font;
    private GameState _gameState = GameState.MainMenu; 
    private Texture2D table;
    private Texture2D Wall;
    private GraphicsDeviceManager _graphics; 
    private SpriteBatch _spriteBatch; 
    private InputState _input; 
    private InventorySystem inventory; 
    
    private Random random = new Random();
    
    private Texture2D pixel;
    
    private CraftGreyboxSystem craftGreyboxSystem;
    
    private TransitionManager transitionManager;
    private Texture2D kepenkTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content"; 
        IsMouseVisible = true; 
        _graphics.PreferredBackBufferWidth = 1920; 
        _graphics.PreferredBackBufferHeight = 1080; 
        _graphics.IsFullScreen = false; 
        _graphics.ApplyChanges(); 
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
        craftingScene.Load(); 

        craftGreyboxSystem = new CraftGreyboxSystem(pixel, font, inventory, Content);

        kepenkTexture = Content.Load<Texture2D>("kepenk");
        transitionManager = new TransitionManager(pixel, kepenkTexture);

        jungleScene = new JungleScene();
        jungleScene.Load(Content, GraphicsDevice, inventory);

        mainMenuScene = new MainMenuScene();
        mainMenuScene.Load(Content);

        // --- KÖPRÜLER BURADA KURULUYOR ---
        
        craftGreyboxSystem.OnSellAttempt = (glass) => 
        {
            craftingScene.OpenSellUI(glass);
        };

        craftingScene.OnSellConfirmed = (glass, price) => 
        {
            craftGreyboxSystem.ResolveSale(glass, price);
        };

        craftGreyboxSystem.OnShopFinished = () =>
        {
            transitionManager.StartTransition(TransitionStyle.Fade, () =>
            {
                jungleScene.ResetDay();
                craftGreyboxSystem.RefreshFromInventory();
                _gameState = GameState.Jungle;
            });
        };
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update(); 

        if (_input.FullScreen()) 
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
        {
            Exit();
        }

        switch (_gameState) 
        {
            case GameState.MainMenu:
                mainMenuScene.Update(gameTime);

                if (mainMenuScene.StartRequested)
                {
                    mainMenuScene.ResetRequest();
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
                    transitionManager.StartTransition(TransitionStyle.Fade, () =>
                    {
                        craftGreyboxSystem.RefreshFromInventory();
                        _gameState = GameState.Crafting;
                    });
                }
                break;

            case GameState.Crafting:
                craftingScene.Update(gameTime);
                craftGreyboxSystem.Update(gameTime);
                break;
        }

        transitionManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) 
    {
        switch (_gameState)
        {
            case GameState.MainMenu:
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                mainMenuScene.Draw(_spriteBatch);
                _spriteBatch.End();
                break;

            case GameState.Jungle:
                GraphicsDevice.Clear(Color.ForestGreen);
                jungleScene.DrawWorld(_spriteBatch, GraphicsDevice.Viewport);
                jungleScene.DrawUi(_spriteBatch, font);
                break;

            case GameState.Crafting:
                GraphicsDevice.Clear(Color.Black);
                
                _spriteBatch.Begin();
                craftGreyboxSystem.Draw(_spriteBatch);
                _spriteBatch.End();

                craftingScene.Draw(_spriteBatch);      
                break;
        }

        _spriteBatch.Begin();
        transitionManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}