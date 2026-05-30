using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PharmaCat.Scripts
{
    public class InputState
    {
        public MouseState _mouseNow, _mousePrev; //these are the srates to track if mouse buttons are clicked or not, we will use this for menu interactions and player movement in jungle scene
        public KeyboardState _kbNow, _kbPrev; // these are the states to track if keys are pressed or not, we will use this for menu interactions and player movement in jungle scene
        private GamePadState _gpNow, _gpPrev; // these are the states to track if gamepad buttons are pressed or not, we will use this for menu interactions and player movement in jungle scene

        public void Update() // this is the update method for input states, we will call this in game1 update method to keep track of input states
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