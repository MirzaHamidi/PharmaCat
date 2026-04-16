using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PharmaCat.Scripts;

namespace PharmaCat;

public class Game1 : Game
{
    private enum GameState
    {
        MainMenu,
        Jungle,
        Shop,
        Crafting,
        Paused,
        GameOver
    }
    private GameState _gameState = GameState.MainMenu;

    private Player player;
    private Texture2D spriteTexture;
    private Vector2 spritePosition;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputState _input;
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
        // TODO: Add your initialization logic here
        _input = new InputState();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(Content.Load<Texture2D>("cat"), Vector2.Zero);

        
        spritePosition = Vector2.Zero;

        
    }

    protected override void Update(GameTime gameTime)
    {
        switch (_gameState)
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
        _input.Update();
        if (_input.FullScreen()) // F4 to toggle fullscreen
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges(); 
        }
    

        base.Update(gameTime);
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        

        base.Update(gameTime);
    }
    private void UpdateJungle(GameTime gameTime)
    {
        if (_input.RightClick())
        {
            player.SetTargetPosition(new Vector2(
                _input._mouseNow.X,
                _input._mouseNow.Y));
        }

    player.Update(gameTime);
    }

    private void UpdateMenu(GameTime gameTime)
    {
        if (_input.LeftClick())
    {
        _gameState = GameState.Jungle;
    }
    }

    private void UpdateShop(GameTime gameTime)
    {
        
    }

    private void UpdateCrafting(GameTime gameTime)
    {
        
    } 

   protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin();

    switch (_gameState)
    {
        case GameState.MainMenu:
            DrawMenu();
            break;

        case GameState.Jungle:
            DrawJungle();
            break;

        case GameState.Shop:
            DrawShop();
            break;

        case GameState.Crafting:
            DrawCrafting();
            break;
    }

    _spriteBatch.End();

    base.Draw(gameTime);
}
    private void DrawMenu()
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
    }

    private void DrawJungle()
    {
        GraphicsDevice.Clear(Color.ForestGreen);
        player.Draw(_spriteBatch);
    }
    private void DrawShop()
    {
        
    }

    private void DrawCrafting()
    {
        
    }

}
