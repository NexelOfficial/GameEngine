using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine
{
    public class Controls
    {
        private static KeyboardState currentKeys;
        private static KeyboardState previousKeys;

        private static MouseState currentMouse;
        private static MouseState previousMouse;

        public static KeyboardState GetState()
        {
            previousKeys = currentKeys;
            currentKeys = Keyboard.GetState();
            return currentKeys;
        }
        public static MouseState GetMouseState()
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            return currentMouse;
        }

        public static bool IsDown(Keys key)
        {
            return currentKeys.IsKeyDown(key);
        }

        public static bool IsPressed(Keys key)
        {
            return currentKeys.IsKeyDown(key) && !previousKeys.IsKeyDown(key);
        }

        public static bool IsDown(string button)
        {
            GetMouseState();

            switch (button)
            {
                case "LeftButton":
                    return currentMouse.LeftButton == ButtonState.Pressed;
                case "RightButton":
                    return currentMouse.RightButton == ButtonState.Pressed;
                case "MiddleButton":
                    return currentMouse.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsPressed(string button)
        {
            GetMouseState();

            switch (button)
            {
                case "LeftButton":
                    return currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
                case "RightButton":
                    return currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released;
                case "MiddleButton":
                    return currentMouse.MiddleButton == ButtonState.Pressed && previousMouse.MiddleButton == ButtonState.Released;
            }
            return false;
        }
    }
}
