using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PharmaCat.Scripts
{
    public class InputState
    {
        public MouseState _mouseNow, _mousePrev; // _mouseNow is current mouse state, _mousePrev is previous frame's state
        public KeyboardState _kbNow, _kbPrev; // _kbNow is current keyboard state, _kbPrev is previous frame's state
        private GamePadState _gpNow, _gpPrev; // _gpNow is current gamepad state, _gpPrev is previous frame's state

        public void Update()
        {
            _mousePrev = _mouseNow; // Store previous mouse state
            _kbPrev = _kbNow; // Store previous keyboard state
            _gpPrev = _gpNow; // Store previous gamepad state

            _mouseNow = Mouse.GetState(); // Get current mouse state
            _kbNow = Keyboard.GetState(); // Get current keyboard state
            _gpNow = GamePad.GetState(PlayerIndex.One); // Get current gamepad state for player one
        }
        public bool KeyDown(Keys k) => _kbNow.IsKeyDown(k);
        public bool KeyPressed(Keys k) => _kbNow.IsKeyDown(k) && !_kbPrev.IsKeyDown(k);
        

        
        public bool ButtonDown(Buttons b) => _gpNow.IsButtonDown(b);
        public bool ButtonPressed(Buttons b) => _gpNow.IsButtonDown(b) && !_gpPrev.IsButtonDown(b);

        public bool RightClick() => _mouseNow.RightButton == ButtonState.Pressed && _mousePrev.RightButton == ButtonState.Released;
        public bool LeftClick() => _mouseNow.LeftButton == ButtonState.Pressed && _mousePrev.LeftButton == ButtonState.Released;
        public bool FullScreen() // Checks if fullscreen toggle key/button was just pressed
        {
            return KeyPressed(Keys.F4);
        }
    }
}