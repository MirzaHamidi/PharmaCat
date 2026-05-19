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
    private GraphicsDeviceManager _graphics; // graphics manager
    private SpriteBatch _spriteBatch; // for drawing sprites
    private InputState _input; // input class call
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
        _input = new InputState(); // initialize input
        base.Initialize();
    }

    protected override void LoadContent()
    {

        font = Content.Load<SpriteFont>("Font");
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        table = Content.Load<Texture2D>("table");
        MyraEnvironment.Game = this;
        jungleScene = new JungleScene();
        jungleScene.Load(Content, GraphicsDevice);
        shopScene = new ShopScene();
        shopScene.Load();
        mainMenuScene = new MainMenuScene();
        mainMenuScene.Load();
        craftingScene = new CraftingScene();
        craftingScene.Load(table);


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

        switch (_gameState) // in here we assigning different updates for different scenes so game logic will be separated and easier to manage, we will add more scenes later like crafting and shop
        {
            case GameState.MainMenu:
                mainMenuScene.Update(gameTime);

                if (mainMenuScene.StartRequested)
                {
                    mainMenuScene.ResetRequest();
                    _gameState = GameState.Jungle;
                }

                break;

            case GameState.Jungle:
                jungleScene.Update(gameTime, _input, GraphicsDevice.Viewport);

                if (jungleScene.CraftingRequested)
                {
                    jungleScene.ResetRequest();
                    _gameState = GameState.Crafting;
                }

                break;

            case GameState.Shop:
                shopScene.Update(gameTime);
                break;

            case GameState.Crafting:
                craftingScene.Update(gameTime);

                if (craftingScene.GoToShopRequested)
                {
                    craftingScene.ResetRequest();
                    _gameState = GameState.Shop;
                }

                break;
        }

        base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime) // in here we assigning different draw calls for different scenes so game rendering will be separated and easier to manage, we will add more scenes later like crafting and shop
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
                _spriteBatch.End();

                break;
        }

        base.Draw(gameTime);
    }




}
