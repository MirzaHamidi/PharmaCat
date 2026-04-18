using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts
{
    internal class Camera2D
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1f;

        public Matrix GetViewMatrix(Viewport viewport)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                Matrix.CreateScale(Zoom, Zoom, 1f) *
                Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0f));
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition, Viewport viewport)
        {
            Vector2 screenCenter = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);
            return Position + (screenPosition - screenCenter) / Zoom;
        }
    }
}