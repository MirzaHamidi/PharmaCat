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
        private float jungleCounter = 25f;
        private int herbCount = 0;

        private TiledIsoEntity pendingCollectionBush = null;

        private Dictionary<string, int> collectedHerbs = new Dictionary<string, int>()
        {
            { "Lavender", 0 },
            { "Blue Lotus", 0 },
            { "Love Rose", 0 },
            { "Anti-Curse Clover", 0 },
            { "Sage", 0 },
            { "Red Poppy", 0 },
            { "Marigold", 0 }
        };

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
                System.IO.File.OpenRead("Content/bitki.png")
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

            UpdatePendingCollection();
            UpdateBushHover(input, viewport);
            UpdateBushVisuals(gameTime);
            RemoveDestroyedBushes();

            JungleEntityUpdater.UpdateEntityAlpha(player, trees, bushes);

            if (input.KeyPressed(Keys.E))
                TryCollectClosestBush();

            UpdateCamera(gameTime);
        }

        private void UpdatePlayerInput(InputState input, Viewport viewport)
        {
            if (input.RightClick())
            {
                Vector2 mouseScreenPos = new Vector2(input._mouseNow.X, input._mouseNow.Y);
                Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, viewport);

                player.SetTargetPosition(mouseWorldPos);
                pendingCollectionBush = null;
                return;
            }

            if (input.LeftClick())
            {
                Vector2 mouseScreenPos = new Vector2(input._mouseNow.X, input._mouseNow.Y);
                Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, viewport);

                TiledIsoEntity clickedBush = FindClickedBush(mouseWorldPos);

                if (clickedBush != null)
                {
                    float distance = Vector2.Distance(player.Position, clickedBush.Position);

                    if (distance <= 60f)
                    {
                        CollectBush(clickedBush);
                        pendingCollectionBush = null;
                    }
                    else
                    {
                        player.SetTargetPosition(clickedBush.Position);
                        pendingCollectionBush = clickedBush;
                    }
                }
                else
                {
                    pendingCollectionBush = null;
                }
            }
        }

        private TiledIsoEntity FindClickedBush(Vector2 mouseWorldPos)
        {
            TiledIsoEntity clickedBush = null;

            foreach (var bush in bushes)
            {
                if (bush.Collected || bush.IsCollecting)
                    continue;

                if (bush.ContainsPoint(mouseWorldPos))
                {
                    if (clickedBush == null || bush.Position.Y > clickedBush.Position.Y)
                    {
                        clickedBush = bush;
                    }
                }
            }

            return clickedBush;
        }

        private void UpdatePendingCollection()
        {
            if (pendingCollectionBush == null)
                return;

            if (pendingCollectionBush.Collected || pendingCollectionBush.IsCollecting)
            {
                pendingCollectionBush = null;
                return;
            }

            float distance = Vector2.Distance(player.Position, pendingCollectionBush.Position);

            if (distance <= 60f)
            {
                CollectBush(pendingCollectionBush);
                pendingCollectionBush = null;
            }
        }

        private void TryCollectClosestBush()
        {
            TiledIsoEntity closestBush = null;
            float closestDist = float.MaxValue;

            foreach (var bush in bushes)
            {
                if (bush.Collected || bush.IsCollecting)
                    continue;

                float dist = Vector2.Distance(player.Position, bush.Position);

                if (dist <= 60f && dist < closestDist)
                {
                    closestDist = dist;
                    closestBush = bush;
                }
            }

            if (closestBush != null)
            {
                CollectBush(closestBush);
                pendingCollectionBush = null;
            }
        }

        private void UpdateBushHover(InputState input, Viewport viewport)
        {
            Vector2 mouseScreenPos = new Vector2(input._mouseNow.X, input._mouseNow.Y);
            Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos, viewport);

            foreach (var bush in bushes)
            {
                if (bush.Collected || bush.IsCollecting)
                {
                    bush.TargetScale = 1f;
                    continue;
                }

                if (bush.ContainsPoint(mouseWorldPos))
                    bush.TargetScale = 1.2f;
                else
                    bush.TargetScale = 1f;
            }
        }

        private void UpdateBushVisuals(GameTime gameTime)
        {
            foreach (var bush in bushes)
            {
                bush.UpdateVisual(gameTime);
            }
        }

        private void RemoveDestroyedBushes()
        {
            bushes.RemoveAll(bush => bush.ShouldRemove);
        }

        private void CollectBush(TiledIsoEntity bush)
        {
            if (bush.Collected || bush.IsCollecting)
                return;

            bush.Collected = true;
            bush.StartCollectAnimation();

            herbCount++;

            string herbName = bush.GetPlantName();

            if (collectedHerbs.ContainsKey(herbName))
                collectedHerbs[herbName]++;
            else
                collectedHerbs[herbName] = 1;
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
                $"Total Herb: {herbCount}",
                new Vector2(1600, 150),
                Color.White
            );

            int uiY = 200;

            foreach (var kvp in collectedHerbs)
            {
                if (kvp.Value > 0)
                {
                    spriteBatch.DrawString(
                        font,
                        $"{kvp.Key}: {kvp.Value}",
                        new Vector2(1600, uiY),
                        Color.White
                    );

                    uiY += 40;
                }
            }

            spriteBatch.End();
        }

        public void ResetRequest()
        {
            CraftingRequested = false;
        }

        public void ResetDay()
        {
            jungleCounter = 25f;
            CraftingRequested = false;
            pendingCollectionBush = null;
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
                tileWidth: 114,
                tileHeight: 125,
                tilesetColumns: 14,
                mapPixelWidth: jungleMapTexture.Width,
                mapPixelHeight: jungleMapTexture.Height
            );
        }
    }
}