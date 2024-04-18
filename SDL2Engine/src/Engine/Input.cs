

namespace SDL2Engine
{
    /// <summary>
    /// Class to handle input from the user
    /// 
    /// Has various methods to check if a key is pressed, released or held down
    /// </summary>
    public static class Input
    {
        private static readonly UInt32[] downKeys = new UInt32[16];
        private static readonly UInt32[] pressedKeys = new UInt32[16];
        private static readonly UInt32[] releasedKeys = new UInt32[16];

        private static Vec2D mousePosition = new();
        private static Vec2D mouseDelta = new();
        private static bool[] mouseButtonsDown = new bool[3];
        private static bool[] mouseButtonsPressed = new bool[3];
        private static bool[] mouseButtonsReleased = new bool[3];

        public static bool GetKeyPressed(UInt32 key)
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

        public static bool GetKeyDown(UInt32 key)
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

        public static bool GetKeyReleased(UInt32 key)
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

        public static void SetKeyDown(UInt32 key)
        {
            for (int i = 0; i < downKeys.Length; i++)
            {
                if (downKeys[i] == 0)
                {
                    downKeys[i] = (UInt32)key;
                    break;
                }
            }
        }

        public static void SetKeyPressed(UInt32 key)
        {
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                if (pressedKeys[i] == 0)
                {
                    pressedKeys[i] = (UInt32)key;
                    break;
                }
            }

            // set key in downKeys
            SetKeyDown(key);
        }

        public static void SetKeyReleased(UInt32 key)
        {
            for (int i = 0; i < releasedKeys.Length; i++)
            {
                if (releasedKeys[i] == 0)
                {
                    releasedKeys[i] = (UInt32)key;
                    break;
                }
            }

            // remove key from downKeys
            for (int i = 0; i < downKeys.Length; i++)
            {
                if (downKeys[i] == key)
                {
                    downKeys[i] = 0;
                    break;
                }
            }
        }

        public static void ClearInputs()
        {
            /*
             * downKeys are reset when the key is released
            for (int i = 0; i < downKeys.Length; i++)
            {
                downKeys[i] = 0;
            }
            */

            for (int i = 0; i < pressedKeys.Length; i++)
            {
                pressedKeys[i] = 0;
            }

            for (int i = 0; i < releasedKeys.Length; i++)
            {
                releasedKeys[i] = 0;
            }

            /* downKeys are reset when the key is released
            for (int i = 0; i < mouseButtonsDown.Length; i++)
            {
                mouseButtonsDown[i] = false;
            }
            */

            for (int i = 0; i < mouseButtonsPressed.Length; i++)
            {
                mouseButtonsPressed[i] = false;
            }

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
        }

        public static void SetMouseButtonPressed(int button)
        {
            if (button < 0 || button >= mouseButtonsPressed.Length)
            {
                return;
            }
            mouseButtonsPressed[button] = true;
            // set button as down
            SetMouseButtonDown(button);
        }

        public static void SetMouseButtonReleased(int button)
        {
            if (button < 0 || button >= mouseButtonsReleased.Length)
            {
                return;
            }
            mouseButtonsReleased[button] = true;
            mouseButtonsDown[button] = false;
        }

        public static void SetMousePosition(Vec2D position)
        {
            mouseDelta = position - mousePosition;
            mousePosition = position;
        }


    }
}
