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
    class PongSquare : Drawable
    {
        public override void Draw(Camera camera)
        {
            var Squaresize = 50;
            var renderer = Engine.renderer;

            var root = this.gameObject;

            // Set the color to white
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);

            // Define the inner rectangle (empty area) with root as center
            Vec2D topLeft = new Vec2D(-Squaresize / 2, -Squaresize / 2) + root.transform.position;
            Vec2D bottomRight = new Vec2D(Squaresize / 2, Squaresize / 2) + root.transform.position;

            // transform to camera space
            topLeft = camera.WorldToScreen(topLeft);
            bottomRight = camera.WorldToScreen(bottomRight);

            // create SDL_Rect
            var InnerRect = new SDL_Rect();
            InnerRect.x = (int)topLeft.x;
            InnerRect.y = (int)topLeft.y;
            InnerRect.w = (int)(bottomRight.x - topLeft.x);
            InnerRect.h = (int)(bottomRight.y - topLeft.y);

            // Draw the inner rectangle (empty area)
            SDL_RenderFillRect(renderer, ref InnerRect);

        }
    }
}
