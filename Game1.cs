using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PharmaCat.Scripts;

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
    private GameState _gameState = GameState.MainMenu; // start at main menu
    private Texture2D jungleMapTexture; // this is the test bg picture, we will replace it with procedural generated map later
    private Player player; // player
    private Camera2D camera; // camera for jungle scene
    private float targetZoom = 1f; // camera zoom
    private Vector2 spritePosition; 
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
        camera = new Camera2D(); // initialize camera
        _input = new InputState(); // initialize input
        base.Initialize(); 
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice); // initialize sprite batch for drawing
        player = new Player(Content.Load<Texture2D>("cat"), Vector2.Zero); // initialize player with texture and position
        jungleMapTexture = Content.Load<Texture2D>("mapjungle"); // load jungle map texture
        
        spritePosition = Vector2.Zero; // initialize sprite position

        //Loadcontent is for preparing assets for the game this is the current assets can be used in game
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
    private void UpdateJungle(GameTime gameTime) //jungles update logic
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

    private void UpdateMenu(GameTime gameTime) // main menu update logic
    {
        if (_input.LeftClick())
    {
        _gameState = GameState.Jungle;
    }
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
    }

    private void DrawJungle() // jungle draw logic
    {
        jungleMapTexture = Content.Load<Texture2D>("mapjungle");
        _spriteBatch.Draw(jungleMapTexture, Vector2.Zero, Color.White);
        player.Draw(_spriteBatch);
    }
    private void DrawShop() // shop draw logic
    {
        
    }

    private void DrawCrafting() // crafting draw logic
    {
        
    }

}
