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

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update();
        if (_input.FullScreen()) // F4 to toggle fullscreen
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges(); 
        }
        if (_input.RightClick())
        {
            player.SetTargetPosition(new Vector2(
                _input._mouseNow.X,
                _input._mouseNow.Y));
        }

        player.Update(gameTime, _input);
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        player.Draw(_spriteBatch);
        _spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
