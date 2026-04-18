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
    private Texture2D jungleMapTexture;
    private Player player;
    private Camera2D camera;
    private float targetZoom = 1f;
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
        camera = new Camera2D();
        _input = new InputState();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(Content.Load<Texture2D>("cat"), Vector2.Zero);
        jungleMapTexture = Content.Load<Texture2D>("mapjungle");
        
        spritePosition = Vector2.Zero;

        
    }

    protected override void Update(GameTime gameTime)
{
    _input.Update();

    if (_input.FullScreen())
    {
        _graphics.IsFullScreen = !_graphics.IsFullScreen;
        _graphics.ApplyChanges();
    }

    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
        Keyboard.GetState().IsKeyDown(Keys.Escape))
    {
        Exit();
    }

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

    base.Update(gameTime);
}
    private void UpdateJungle(GameTime gameTime)
    {
        int scrollDelta = _input.MouseScrollDelta();

        if (scrollDelta > 0)
        {
            targetZoom += 0.1f;
        }
        
        else if (scrollDelta < 0)
        {
            targetZoom -= 0.1f;
        }

        targetZoom = MathHelper.Clamp(targetZoom, 0.8f, 1.4f);

        camera.Zoom = MathHelper.Lerp(
            camera.Zoom,
            targetZoom,
            8f * (float)gameTime.ElapsedGameTime.TotalSeconds);

        if (_input.RightClick())
        {
            Vector2 mouseScreenPos = new Vector2(_input._mouseNow.X, _input._mouseNow.Y);
            Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, GraphicsDevice.Viewport);

            player.SetTargetPosition(mouseWorldPos);
        }

        player.Update(gameTime);

        float smoothSpeed = 5f;
        camera.Position = Vector2.Lerp(
            camera.Position,
            player.Position,
            smoothSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds
        );
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
    private void DrawMenu()
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
    }

    private void DrawJungle()
    {
        jungleMapTexture = Content.Load<Texture2D>("mapjungle");
        _spriteBatch.Draw(jungleMapTexture, Vector2.Zero, Color.White);
        player.Draw(_spriteBatch);
    }
    private void DrawShop()
    {
        
    }

    private void DrawCrafting()
    {
        
    }

}
