using SDL2;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace Pong.src
{
    internal class PongGame
    {
        public static Scene CreateScene()
        {


            var scene = new Scene("PongGame Scene");

            // var leftPaddle = scene.CreateChild("Left Paddle");
            var leftPaddle = new GameObject("Left Paddle");
            _ = leftPaddle.AddComponent<WSController>();
            _ = leftPaddle.AddComponent<Paddle>();
            leftPaddle.SetPosition(new Vec2D(50, 750));
            scene.AddGameObject(leftPaddle);


            var rightPaddle = new GameObject("Right Paddle");
            _ = rightPaddle.AddComponent<ArrowKeysController>();
            _ = rightPaddle.AddComponent<Paddle>();
            rightPaddle.transform.position = new Vec2D(1870, 750);
            scene.AddGameObject(rightPaddle);

            ////PongBall variante round
            //var pongBall = scene.CreateChild("Pongball");
            ////_ = pongBall.AddComponent<ArrowKeysController>();
            //_ = pongBall.AddComponent<PongBall>();
            //pongBall.transform.position = new Vec2D(950, 750);

            ////PongBall variante round
            var pongSquare = new GameObject("PongSquare");
            //_ = pongBall.AddComponent<ArrowKeysController>();
            _ = pongSquare.AddComponent<PongSquare>();
            //pongSquare.transform.position = new Vec2D(960, 750);
            var bc = pongSquare.AddComponent<BoxCollider>();
            var pb = pongSquare.AddComponent<PhysicsBody>();
            if (pb != null)
            {
                var rand = new Random();
                pb.Velocity = new Vec2D(rand.Next(-500, 500), rand.Next(-500, 500));
                pb.Drag = 0d;
            }
            pongSquare.SetPosition(new Vec2D(960, 750));
            scene.AddGameObject(pongSquare);

            ////Boarder variante 1 
            //var leftBoarder = scene.CreateChild("Boarder");
            //_ = leftBoarder.AddComponent<Boarder>();
            //leftBoarder.transform.position = new Vec2D(5, 750);

            //var rightBoarder = scene.CreateChild("Boarder");
            //_ = rightBoarder.AddComponent<Boarder>();
            //rightBoarder.transform.position = new Vec2D(1915, 750);

            //Boarder variante 2
            var testBoarder = new GameObject("Boarder");
            _ = testBoarder.AddComponent<Boarder2>();
            testBoarder.transform.position = new Vec2D(960, 750);
            scene.AddGameObject(testBoarder);

            //testBoarder.transform.position = new Vec2D(scene.GetCamera().GetWorldSize().x / 2, scene.GetCamera().GetWorldSize().y / 2);

            // Text erstellen und rendern
            //var helloText = scene.CreateChild("Hello Text");
            //_ = testBoarder.AddComponent<Text>();
            //helloText.transform.position = new Vec2D(960, 300);
            // RenderText("Hello!", helloText.transform.position, Engine.renderer, scene.GetCamera());

            return scene;
        }
        public static void Run()
        {

            var scene = CreateScene();

            var engine = new SDL2Engine.Engine(scene);

            engine.Run();
        }

        // Methode zum Rendern des Textes
        //private static void RenderText(string text, Vec2D worldPosition, nint renderer, Camera camera)
        //{

        //    Console.WriteLine(camera);
        //    nint sans = TTF_OpenFont("Sans.ttf", 24);

        //    SDL_Color white = new();
        //    white.r = white.g = white.b = white.a = 255;

        //    nint surfaceMessage = TTF_RenderText_Solid(sans, text, white);

        //    // now you can convert it into a texture
        //    nint message = SDL_CreateTextureFromSurface(renderer, surfaceMessage);

        //    SDL_Rect message_rect;
        //    message_rect.w = message_rect.h = 0;

        //    // Convert world position to screen position using camera
        //    Vec2D screenPosition = camera.WorldToScreen(worldPosition);
        //    message_rect.x = (int)screenPosition.x;
        //    message_rect.y = (int)screenPosition.y;

        //    // Get the size of the text
        //    SDL_QueryTexture(message, out _, out _, out message_rect.w, out message_rect.h);

        //    SDL_RenderCopy(renderer, message, (nint)null, ref message_rect);

        //    SDL_FreeSurface(surfaceMessage);
        //    SDL_DestroyTexture(message);
        //    TTF_CloseFont(sans);
        //}

    }
}


class ArrowKeysController : Script
{
    public override void Update()
    {

        var gameObject = this.gameObject;
        var speed = 10;
        if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_UP)))
        {
            gameObject.transform.position += new Vec2D(0, -speed);
        }
        if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_DOWN)))
        {
            gameObject.transform.position += new Vec2D(0, speed);
        }

    }
}


class WSController : Script
{
    public override void Update()
    {
        // a better way to do this would be to use a rigidbody with velocity
        var gameObject = this.gameObject;
        var speed = 10;
        if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_w)))
        {
            gameObject.transform.position += new Vec2D(0, -speed);
        }
        if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_s)))
        {
            gameObject.transform.position += new Vec2D(0, speed);
        }

    }
}