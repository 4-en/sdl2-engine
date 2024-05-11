using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Utils
{
    public class MouseHelper
    {
        public static bool IsRectHovered(Rect rect)
        {
            var mousePos = Input.GetMousePosition();
            return mousePos.x >= rect.x && mousePos.x <= rect.x + rect.w && mousePos.y >= rect.y && mousePos.y <= rect.y + rect.h;
        }

        public static bool IsRectClicked(Rect rect, int button=0)
        {
            return Input.GetMouseButtonDown(button) && IsRectHovered(rect);
        }

        public static bool IsRectPressed(Rect rect, int button=0)
        {
            return Input.GetMouseButtonPressed(button) && IsRectHovered(rect);
        }
        
        public static bool IsRectReleased(Rect rect, int button=0)
        {
            return Input.GetMouseButtonReleased(button) && IsRectHovered(rect);
        }

    }
}
