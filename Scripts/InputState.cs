using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PharmaCat.Scripts
{
    public class InputState
    {
        public MouseState _mouseNow, _mousePrev;
        public KeyboardState _kbNow, _kbPrev;
        private GamePadState _gpNow, _gpPrev;

        public void Update()
        {
            _mousePrev = _mouseNow;
            _kbPrev = _kbNow;
            _gpPrev = _gpNow;

            _mouseNow = Mouse.GetState();
            _kbNow = Keyboard.GetState();
            _gpNow = GamePad.GetState(PlayerIndex.One);
        }

        public bool KeyDown(Keys k) => _kbNow.IsKeyDown(k);
        public bool KeyPressed(Keys k) => _kbNow.IsKeyDown(k) && !_kbPrev.IsKeyDown(k);

        public bool ButtonDown(Buttons b) => _gpNow.IsButtonDown(b);
        public bool ButtonPressed(Buttons b) => _gpNow.IsButtonDown(b) && !_gpPrev.IsButtonDown(b);

        public bool RightClick() => _mouseNow.RightButton == ButtonState.Pressed && _mousePrev.RightButton == ButtonState.Released;
        public bool LeftClick() => _mouseNow.LeftButton == ButtonState.Pressed && _mousePrev.LeftButton == ButtonState.Released;

        public int MouseScrollDelta()
        {
            return _mouseNow.ScrollWheelValue - _mousePrev.ScrollWheelValue;
        }

        public bool FullScreen()
        {
            return KeyPressed(Keys.F4);
        }
    }
}