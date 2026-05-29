using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    public enum TransitionStyle
    {
        Fade,
        Shutter
    }

    public class TransitionManager
    {
        private Texture2D pixel;
        private Texture2D kepenkTexture;

        public bool IsTransitioning { get; private set; }
        private bool isCovering = false;
        
        private TransitionStyle currentStyle;
        private Action onTransitionMidpoint;

        private float alpha = 0f; 
        private float kepenkY = -1080f; 

        public TransitionManager(Texture2D pixel, Texture2D kepenkTexture)
        {
            this.pixel = pixel;
            this.kepenkTexture = kepenkTexture;
        }

        public void StartTransition(TransitionStyle style, Action onMidpoint)
        {
            if (IsTransitioning) return;

            currentStyle = style;
            onTransitionMidpoint = onMidpoint;
            IsTransitioning = true;
            isCovering = true;

            if (style == TransitionStyle.Fade) alpha = 0f;
            if (style == TransitionStyle.Shutter) kepenkY = -1080f;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsTransitioning) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            float fadeSpeed = 1.5f; 
            float shutterSpeed = 1800f; 

            if (isCovering) 
            {
                if (currentStyle == TransitionStyle.Fade)
                {
                    alpha += fadeSpeed * dt;
                    if (alpha >= 1f) { alpha = 1f; TriggerMidpoint(); }
                }
                else if (currentStyle == TransitionStyle.Shutter)
                {
                    kepenkY += shutterSpeed * dt;
                    if (kepenkY >= 0f) { kepenkY = 0f; TriggerMidpoint(); } 
                }
            }
            else 
            {
                if (currentStyle == TransitionStyle.Fade)
                {
                    alpha -= fadeSpeed * dt;
                    if (alpha <= 0f) { alpha = 0f; IsTransitioning = false; } 
                }
                else if (currentStyle == TransitionStyle.Shutter)
                {
                    kepenkY -= shutterSpeed * dt; 
                    if (kepenkY <= -1080f) { kepenkY = -1080f; IsTransitioning = false; } 
                }
            }
        }

        private void TriggerMidpoint()
        {
            onTransitionMidpoint?.Invoke(); 
            isCovering = false; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsTransitioning) return;

            if (currentStyle == TransitionStyle.Fade)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 1920, 1080), Color.Black * alpha);
            }
            else if (currentStyle == TransitionStyle.Shutter)
            {
                spriteBatch.Draw(kepenkTexture, new Rectangle(0, (int)kepenkY, 1920, 1080), Color.White);
            }
        }
    }
}