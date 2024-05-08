using SDL2;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Pong.src
{
    class Paddle : Drawable
    {

        public override void Draw(Camera camera)
        {
            var PaddleWidth = 35;
            var PaddleHeight = 160;

            var renderer = Engine.renderer;

            var root = this.gameObject;

            anchorPoint = AnchorPoint.TopLeft;


            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);


            // define the square
            Rect rect = new Rect(PaddleHeight, PaddleWidth);
            rect = camera.RectToScreen(rect, root.transform.position);
            var sdl_rect = rect.ToSDLRect();

            SDL_RenderFillRect(renderer, ref sdl_rect);
        }
    }
}

