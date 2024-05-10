

using static SDL2.SDL;

namespace SDL2Engine
{
    /// <summary>
    /// Class to handle input from the user
    /// 
    /// Has various methods to check if a key is pressed, released or held down
    /// </summary>
    public static class Input
    {
        private static readonly int[] downKeys = new int[16];
        private static readonly int[] pressedKeys = new int[16];
        private static readonly int[] releasedKeys = new int[16];

        private static Vec2D mousePosition = new();
        private static Vec2D mouseDelta = new();
        private static bool[] mouseButtonsDown = new bool[3];
        private static bool[] mouseButtonsPressed = new bool[3];
        private static bool[] mouseButtonsReleased = new bool[3];

        public static bool GetKeyPressed(int key)
        {
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                if (pressedKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetKeyPressed(SDL_Keycode key)
        {
            return GetKeyPressed((int)key);
        }

        public static bool GetKeyDown(int key)
        {
            for (int i = 0; i < downKeys.Length; i++)
            {
                if (downKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetKeyDown(SDL_Keycode key)
        {
            return GetKeyDown((int)key);
        }

        public static bool GetKeyReleased(int key)
        {
            for (int i = 0; i < releasedKeys.Length; i++)
            {
                if (releasedKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetKeyReleased(SDL_Keycode key)
        {
            return GetKeyReleased((int)key);
        }

        public static void SetKeyDown(int key)
        {
            for (int i = 0; i < downKeys.Length; i++)
            {
                if (downKeys[i] == 0)
                {
                    downKeys[i] = key;
                    break;
                }
            }

            SetKeyPressed(key);
        }

        public static void SetKeyDown(SDL_Keycode key)
        {
            SetKeyDown((int)key);
        }

        public static void SetKeyPressed(int key)
        {
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                if (pressedKeys[i] == 0)
                {
                    pressedKeys[i] = key;
                    break;
                }
            }

        }

        public static void SetKeyPressed(SDL_Keycode key)
        {
            SetKeyPressed((int)key);
        }

        public static void SetKeyReleased(int key)
        {
            for (int i = 0; i < releasedKeys.Length; i++)
            {
                if (releasedKeys[i] == 0)
                {
                    releasedKeys[i] = key;
                    break;
                }
            }

            // remove key from pressedKeys
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                if (pressedKeys[i] == key)
                {
                    pressedKeys[i] = 0;
                    break;
                }
            }
        }

        public static void SetKeyReleased(SDL_Keycode key)
        {
            SetKeyReleased((int)key);
        }

        public static void ClearInputs()
        {
            
            for (int i = 0; i < downKeys.Length; i++)
            {
                downKeys[i] = 0;
            }

            /*
             * pressedKeys are reset when the key is released
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                pressedKeys[i] = 0;
            }
            */

            for (int i = 0; i < releasedKeys.Length; i++)
            {
                releasedKeys[i] = 0;
            }

            
            for (int i = 0; i < mouseButtonsDown.Length; i++)
            {
                mouseButtonsDown[i] = false;
            }

            /*
             * pressedKeys are reset when the key is released
            for (int i = 0; i < mouseButtonsPressed.Length; i++)
            {
                mouseButtonsPressed[i] = false;
            }
            */

            for (int i = 0; i < mouseButtonsReleased.Length; i++)
            {
                mouseButtonsReleased[i] = false;
            }
        }

        public static Vec2D GetMousePosition()
        {
            return mousePosition;
        }

        public static Vec2D GetMouseDelta()
        {
            return mouseDelta;
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (button < 0 || button >= mouseButtonsDown.Length)
            {
                return false;
            }
            return mouseButtonsDown[button];
        }

        public static bool GetMouseButtonPressed(int button)
        {
            if (button < 0 || button >= mouseButtonsPressed.Length)
            {
                return false;
            }
            return mouseButtonsPressed[button];
        }

        public static bool GetMouseButtonReleased(int button)
        {
            if (button < 0 || button >= mouseButtonsReleased.Length)
            {
                return false;
            }
            return mouseButtonsReleased[button];
        }

        public static void SetMouseButtonDown(int button)
        {
            if (button < 0 || button >= mouseButtonsDown.Length)
            {
                return;
            }
            mouseButtonsDown[button] = true;
            SetMouseButtonPressed(button);
        }

        public static void SetMouseButtonPressed(int button)
        {
            if (button < 0 || button >= mouseButtonsPressed.Length)
            {
                return;
            }
            mouseButtonsPressed[button] = true;
        }

        public static void SetMouseButtonReleased(int button)
        {
            if (button < 0 || button >= mouseButtonsReleased.Length)
            {
                return;
            }
            mouseButtonsReleased[button] = true;
            mouseButtonsPressed[button] = false;
        }

        public static void SetMousePosition(Vec2D position)
        {
            mouseDelta = position - mousePosition;
            mousePosition = position;
        }


    }
}
