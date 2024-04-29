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
            var PaddleSpeed = 10;

            var renderer = Engine.renderer;

            var root = this.gameObject;


            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);


            // define the square
            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(-PaddleWidth / 2, -PaddleHeight / 2));
            points.Add(new Vec2D(PaddleWidth / 2, -PaddleHeight / 2));
            points.Add(new Vec2D(PaddleWidth / 2, PaddleHeight / 2));
            points.Add(new Vec2D(-PaddleWidth / 2, PaddleHeight / 2));

            // convert to camera space
            for (int i = 0; i < points.Count; i++)
            {
                // rotate around center
                Vec2D p = points[i];
                p = camera.WorldToScreen(p, root.GetPosition());
                points[i] = p;
            }

            // draw the filled white square
            var rect = new SDL_Rect();
            rect.x = (int)points[0].x;
            rect.y = (int)points[0].y;
            rect.w = (int)(points[1].x - points[0].x);
            rect.h = (int)(points[2].y - points[1].y);
            SDL_RenderFillRect(renderer, ref rect);
        }
    }
}

