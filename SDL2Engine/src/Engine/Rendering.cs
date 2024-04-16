using SDL2;
using static SDL2.SDL;
using System;

namespace SDL2Engine
{

    // Base class for all drawable components
    // these components are called by their GameObjects to draw themselves
    public class Drawable : Component
    {

        public Drawable(GameObject gameObject) : base(gameObject)
        {
        }

        public virtual void Draw(ICamera camera)
        {
            throw new NotImplementedException();
        }
    }


    public class RotatingSquare : Drawable
    {

        public double rotationsPerSecond = 0.3;
        public RotatingSquare(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Draw(ICamera camera)
        {
            // draw a square that rotates around its center
            var renderer = Engine.renderer;
            double time = Time.time;
            double angle = time * 2 * Math.PI * rotationsPerSecond;

            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);

            Vec2D center = new Vec2D(0, 0);
            center = camera.WorldToScreen(center);
            // define the square
            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(-250, -250));
            points.Add(new Vec2D(250, -250));
            points.Add(new Vec2D(250, 250));
            points.Add(new Vec2D(-250, 250));

            // rotate the square
            for (int i = 0; i < points.Count; i++)
            {
                // rotate around center
                Vec2D p = points[i];
                p = camera.WorldToScreen(p);
                double x = p.x - center.x;
                double y = p.y - center.y;
                points[i] = new Vec2D(
                    x * Math.Cos(angle) - y * Math.Sin(angle),
                    x * Math.Sin(angle) + y * Math.Cos(angle)
                );
            }

            // draw the square
            for (int i = 0; i < points.Count; i++)
            {
                Vec2D p1 = points[i];
                Vec2D p2 = points[(i + 1) % points.Count];
                SDL_RenderDrawLine(renderer, (int)(p1.x + center.x), (int)(p1.y + center.y), (int)(p2.x + center.x), (int)(p2.y + center.y));
            }

            
        }
    }
}
