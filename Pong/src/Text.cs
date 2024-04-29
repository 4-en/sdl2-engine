using SDL2;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static System.Net.Mime.MediaTypeNames;

namespace Pong.src
{
    class Text : Drawable
    {
        public override void Draw(Camera camera)
        {
            //Console.WriteLine("TEXT");
            var renderer = Engine.renderer;

            nint Sans = SDL_ttf.TTF_OpenFont("Sans.ttf", 24);

            SDL_Color White = new();
            White.r = White.g = White.b = White.a = 255;

            nint surfaceMessage = SDL_ttf.TTF_RenderText_Solid(Sans, "put your text here", White);

            // now you can convert it into a texture
            nint Message = SDL_CreateTextureFromSurface(renderer, surfaceMessage);

            SDL_Rect Message_rect;
            Message_rect.x = 0;
            Message_rect.y = 0;
            Message_rect.w = 100;
            Message_rect.h = 100;

            SDL_RenderCopy(renderer, Message, (nint)null, ref Message_rect);

            SDL_FreeSurface(surfaceMessage);
            SDL_DestroyTexture(Message);
        }
    }

}
