using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    internal enum ShopActorState
    {
        Entering,
        IntroTalking,
        Idle,
        ResultTalking,
        WaitingAfterResult,
        Leaving,
        Gone
    }

    internal class ShopDialogueActor
    {
        private Texture2D normalTexture;
        private Texture2D happyTexture;
        private Texture2D angryTexture;
        private Texture2D pixel;
        private SpriteFont font;

        private Vector2 position;
        private Vector2 startPosition;
        private Vector2 centerPosition;
        private Vector2 exitPosition;

        private string fullText = "";
        private string visibleText = "";

        private float moveSpeed = 700f;
        private float typeTimer;
        private float typeSpeed = 0.035f;

        private float faceTimer;
        private float faceSwitchSpeed = 0.12f;
        private bool useSecondFace;

        private float waitTimer;

        private ShopActorState state = ShopActorState.Gone;

        public bool IntroFinished { get; private set; }
        public bool IsGone => state == ShopActorState.Gone;

        public ShopDialogueActor(
            Texture2D normalTexture,
            Texture2D happyTexture,
            Texture2D angryTexture,
            Texture2D pixel,
            SpriteFont font,
            Viewport viewport)
        {
            this.normalTexture = normalTexture;
            this.happyTexture = happyTexture;
            this.angryTexture = angryTexture;
            this.pixel = pixel;
            this.font = font;

            centerPosition = new Vector2(viewport.Width / 2f, viewport.Height / 2f + 170);
            startPosition = new Vector2(-250, centerPosition.Y);
            exitPosition = new Vector2(viewport.Width + 250, centerPosition.Y);

            position = startPosition;
        }

        public void StartIntro(string text)
        {
            fullText = text;
            visibleText = "";

            position = startPosition;
            state = ShopActorState.Entering;

            IntroFinished = false;
            typeTimer = 0f;
            faceTimer = 0f;
            waitTimer = 0f;
            useSecondFace = false;
        }

        public void StartResult(string text)
        {
            fullText = text;
            visibleText = "";

            state = ShopActorState.ResultTalking;

            typeTimer = 0f;
            faceTimer = 0f;
            waitTimer = 0f;
            useSecondFace = false;
        }

        public void ForceLeave()
        {
            state = ShopActorState.Leaving;
            visibleText = "";
            fullText = "";
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (state)
            {
                case ShopActorState.Entering:
                    UpdateEntering(dt);
                    break;

                case ShopActorState.IntroTalking:
                    UpdateTypewriter(dt);

                    if (IsTextFinished())
                    {
                        IntroFinished = true;
                        state = ShopActorState.Idle;
                        useSecondFace = false;
                    }
                    else
                    {
                        UpdateFaceTalking(dt);
                    }

                    break;

                case ShopActorState.Idle:
                    useSecondFace = false;
                    break;

                case ShopActorState.ResultTalking:
                    UpdateTypewriter(dt);

                    if (IsTextFinished())
                    {
                        useSecondFace = false;
                        state = ShopActorState.WaitingAfterResult;
                        waitTimer = 0f;
                    }
                    else
                    {
                        UpdateFaceTalking(dt);
                    }

                    break;

                case ShopActorState.WaitingAfterResult:
                    waitTimer += dt;

                    if (waitTimer >= 1f)
                    {
                        visibleText = "";
                        fullText = "";
                        state = ShopActorState.Leaving;
                    }

                    break;

                case ShopActorState.Leaving:
                    UpdateLeaving(dt);
                    break;
            }
        }

        private void UpdateEntering(float dt)
        {
            position = MoveTowards(position, centerPosition, moveSpeed * dt);

            if (Vector2.Distance(position, centerPosition) < 2f)
            {
                position = centerPosition;
                state = ShopActorState.IntroTalking;
            }
        }

        private void UpdateLeaving(float dt)
        {
            position = MoveTowards(position, exitPosition, moveSpeed * dt);

            if (Vector2.Distance(position, exitPosition) < 2f)
            {
                state = ShopActorState.Gone;
            }
        }

        private void UpdateTypewriter(float dt)
        {
            if (visibleText.Length >= fullText.Length)
                return;

            typeTimer += dt;

            while (typeTimer >= typeSpeed && visibleText.Length < fullText.Length)
            {
                visibleText += fullText[visibleText.Length];
                typeTimer -= typeSpeed;
            }
        }

        private void UpdateFaceTalking(float dt)
        {
            faceTimer += dt;

            if (faceTimer >= faceSwitchSpeed)
            {
                faceTimer = 0f;
                useSecondFace = !useSecondFace;
            }
        }

        private bool IsTextFinished()
        {
            return visibleText.Length >= fullText.Length;
        }

        private Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistance)
        {
            Vector2 direction = target - current;
            float distance = direction.Length();

            if (distance <= maxDistance || distance == 0f)
                return target;

            direction.Normalize();
            return current + direction * maxDistance;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (state == ShopActorState.Gone)
                return;

            Texture2D currentTexture = GetCurrentTexture();

            Vector2 origin = new Vector2(
                currentTexture.Width / 2f,
                currentTexture.Height / 2f
            );

            spriteBatch.Draw(
                currentTexture,
                position,
                null,
                Color.White,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            if (!string.IsNullOrEmpty(visibleText))
            {
                DrawDialogueBox(spriteBatch);
            }
        }

        private Texture2D GetCurrentTexture()
        {
            if (state == ShopActorState.IntroTalking)
            {
                return useSecondFace ? happyTexture : normalTexture;
            }

            if (state == ShopActorState.ResultTalking)
            {
                return useSecondFace ? angryTexture : normalTexture;
            }

            return normalTexture;
        }

        private void DrawDialogueBox(SpriteBatch spriteBatch)
        {
            Rectangle box = new Rectangle(520, 680, 880, 190);

            spriteBatch.Draw(pixel, box, new Color(0, 0, 0, 210));
            spriteBatch.Draw(pixel, new Rectangle(box.X, box.Y, box.Width, 4), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(box.X, box.Bottom - 4, box.Width, 4), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(box.X, box.Y, 4, box.Height), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(box.Right - 4, box.Y, 4, box.Height), Color.White);

            string wrappedText = WrapText(visibleText, 780);

            spriteBatch.DrawString(
                font,
                wrappedText,
                new Vector2(box.X + 35, box.Y + 35),
                Color.White
            );
        }

        private string WrapText(string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            string result = "";
            string line = "";

            foreach (string word in words)
            {
                string testLine = line.Length == 0 ? word : line + " " + word;
                Vector2 size = font.MeasureString(testLine);

                if (size.X > maxLineWidth)
                {
                    result += line + "\n";
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }

            result += line;
            return result;
        }
    }
}