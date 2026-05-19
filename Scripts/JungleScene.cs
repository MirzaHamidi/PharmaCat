using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PharmaCat.Scripts.World;
using PharmaCat.Scripts.Rendering;

namespace PharmaCat.Scripts
{
    internal class JungleScene
    {
        private Texture2D jungleMapTexture;
        private Texture2D treeTexture;
        private Texture2D bushTexture;

        private List<TiledIsoEntity> trees = new List<TiledIsoEntity>();
        private List<TiledIsoEntity> bushes = new List<TiledIsoEntity>();

        private Player player;
        private Camera2D camera;

        private float targetZoom = 1f;
        private bool cameraInitialized = false;
        private float jungleCounter = 50f;
        private int herbCount = 0;

        public bool CraftingRequested { get; private set; }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            jungleMapTexture = content.Load<Texture2D>("mapjungle");

            treeTexture = Texture2D.FromStream(
            graphicsDevice,
            System.IO.File.OpenRead("Content/treeset.png")
            );

            bushTexture = Texture2D.FromStream(
            graphicsDevice,
            System.IO.File.OpenRead("Content/treeset.png")
            );

            Vector2 mapCenter = new Vector2(
                jungleMapTexture.Width / 2f,
                jungleMapTexture.Height / 2f
            );

            player = new Player(content.Load<Texture2D>("cat"), mapCenter);
            camera = new Camera2D();

            CreateWorldEntities();
        }

        public void Update(GameTime gameTime, InputState input, Viewport viewport)
        {
            UpdateCounter(gameTime);
            UpdateZoom(gameTime, input);
            UpdatePlayerInput(input, viewport);

            player.Update(gameTime);

            JungleEntityUpdater.UpdateEntityAlpha(player, trees, bushes);

            if (input.KeyPressed(Keys.E))
            {
                if (JungleEntityUpdater.TryCollectBush(player, bushes))
                    herbCount++;
            }

            UpdateCamera(gameTime);
        }

        public void DrawWorld(SpriteBatch spriteBatch, Viewport viewport)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(viewport));

            spriteBatch.Draw(jungleMapTexture, Vector2.Zero, Color.White);
            YSortRenderer.Draw(spriteBatch, player, trees, bushes);

            spriteBatch.End();
        }

        public void DrawUi(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(
                font,
                $"{Math.Ceiling(jungleCounter)}",
                new Vector2(1600, 100),
                Color.White
            );

            spriteBatch.DrawString(
                font,
                $"Herb: {herbCount}",
                new Vector2(1600, 150),
                Color.White
            );

            spriteBatch.End();
        }

        public void ResetRequest()
        {
            CraftingRequested = false;
        }

        private void UpdateCounter(GameTime gameTime)
        {
            if (jungleCounter <= 0)
                return;

            jungleCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (jungleCounter < 0)
            {
                jungleCounter = 0f;
                CraftingRequested = true;
            }
        }

        private void UpdateZoom(GameTime gameTime, InputState input)
        {
            int scrollDelta = input.MouseScrollDelta();

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
        }

        private void UpdatePlayerInput(InputState input, Viewport viewport)
        {
            if (!input.RightClick())
                return;

            Vector2 mouseScreenPos = new Vector2(input._mouseNow.X, input._mouseNow.Y);
            Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, viewport);

            player.SetTargetPosition(mouseWorldPos);
        }

        private void UpdateCamera(GameTime gameTime)
        {
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

        private void CreateWorldEntities()
{
    trees.Clear();
    bushes.Clear();

    IsoEntityLoader.LoadEntityLayerFromCsv(
        "Content/bitki_agac.csv",
        treeTexture,
        trees,
        false,

        firstGid: 0,
        tileWidth: 122,
        tileHeight: 150,
        tilesetColumns: 12,
        mapPixelWidth: jungleMapTexture.Width,
        mapPixelHeight: jungleMapTexture.Height
    );

    IsoEntityLoader.LoadEntityLayerFromCsv(
        "Content/bitki_cicek.csv",
        bushTexture,
        bushes,
        true,

        firstGid: 0,
        tileWidth: 122,
        tileHeight: 150,
        tilesetColumns: 12,
        mapPixelWidth: jungleMapTexture.Width,
        mapPixelHeight: jungleMapTexture.Height
    );
}
    }
}