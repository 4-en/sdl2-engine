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
    class PongBall : Drawable
    {
        public override void Draw(Camera camera)
        {
            var PongBallRadius = 15; // Radius des Balls

            var renderer = Engine.renderer;
            var root = this.gameObject;

            // Position des Balls in Weltkoordinaten
            Vec2D center = new Vec2D(0, 0);
            center = camera.WorldToScreen(center, root.GetPosition());

            // draw the circle
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255); // Farbe auf Weiß setzen

            SDL_RenderDrawPoint(renderer, (int)center.x, (int)center.y); // Mittelpunkt zeichnen

            // Feinere Kreiszeichnung
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255); // Farbe auf Weiß setzen

            for (float theta = 0; theta < (2 * Math.PI); theta += 0.01f) // Schleife für die Kreiszeichnung mit feinerer Schrittweite
            {
                int x = (int)(center.x + PongBallRadius * Math.Cos(theta)); // Berechnung der x-Koordinate des Kreispunktes
                int y = (int)(center.y + PongBallRadius * Math.Sin(theta)); // Berechnung der y-Koordinate des Kreispunktes

                SDL_RenderDrawPoint(renderer, x, y); // Kreispunkt zeichnen
            }
        }
    }
}
