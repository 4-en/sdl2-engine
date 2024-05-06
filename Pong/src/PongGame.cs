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
            var leftPaddle = new GameObject("LeftPaddle");
            _ = leftPaddle.AddComponent<WSController>();
            _ = leftPaddle.AddComponent<Paddle>();
            leftPaddle.AddComponent<PhysicsBody>();
            var leftPaddleBoxCollider = leftPaddle.AddComponent<BoxCollider>();
            leftPaddleBoxCollider.UpdateColliderPosition(new Vec2D(50, 750));
            leftPaddleBoxCollider.UpdateColliderSize(35, 160);
            leftPaddle.transform.position = new Vec2D(50, 750);
            scene.AddGameObject(leftPaddle);


            var rightPaddle = new GameObject("RightPaddle");
            _ = rightPaddle.AddComponent<ArrowKeysController>();
            _ = rightPaddle.AddComponent<Paddle>();
            rightPaddle.AddComponent<PhysicsBody>();
            var rightPaddleBoxCollider = rightPaddle.AddComponent<BoxCollider>();
            rightPaddleBoxCollider.UpdateColliderPosition(new Vec2D(1870, 750));
            rightPaddleBoxCollider.UpdateColliderSize(35, 160);
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
            pb.Velocity = new Vec2D(8, 5);
            //pongSquare.SetPosition(new Vec2D(960, 750));
            pongSquare.SetPosition(new Vec2D(960- 480-50, 750));
            scene.AddGameObject(pongSquare);

            ////Boarder variante 1 
            //var leftBoarder = scene.CreateChild("Boarder");
            //_ = leftBoarder.AddComponent<Boarder>();
            //leftBoarder.transform.position = new Vec2D(5, 750);

            //var rightBoarder = scene.CreateChild("Boarder");
            //_ = rightBoarder.AddComponent<Boarder>();
            //rightBoarder.transform.position = new Vec2D(1915, 750);

            //Boarder variante 2
            var testBoarder = new GameObject("BoarderBottom");
            _ = testBoarder.AddComponent<Boarder2>();
            var bordercollider = testBoarder.AddComponent<BoxCollider>();
            bordercollider.UpdateColliderPosition(new Vec2D(0, 750 + 550));
            bordercollider.UpdateColliderSize(1980,5);
            var borderphysics = testBoarder.AddComponent<PhysicsBody>();
            borderphysics.IsMovable = false;
            borderphysics.Mass = 0.5;
            testBoarder.transform.position = new Vec2D(960, 750);
            scene.AddGameObject(testBoarder);


            //Boarder top for test
            var testBoarder2 = new GameObject("BoarderTop");
            _ = testBoarder2.AddComponent<Boarder2>();
            var bordercollider2 = testBoarder2.AddComponent<BoxCollider>();
            bordercollider2.UpdateColliderPosition(new Vec2D(0, 223));
            bordercollider2.UpdateColliderSize(1980, 5);
            var borderphysics2 = testBoarder2.AddComponent<PhysicsBody>();
            borderphysics2.IsMovable = false;
            borderphysics2.Mass = 0.5;
            testBoarder2.transform.position = new Vec2D(960, 750);
            scene.AddGameObject(testBoarder2);


            //Boarder left for test
            var testBoarder3 = new GameObject("BoarderLeft");
            _ = testBoarder3.AddComponent<Boarder2>();
            var bordercollider3 = testBoarder3.AddComponent<BoxCollider>();
            bordercollider3.UpdateColliderPosition(new Vec2D(5, 228));
            bordercollider3.UpdateColliderSize( 5, 1070);
            var borderphysics3 = testBoarder3.AddComponent<PhysicsBody>();
            borderphysics3.IsMovable = false;
            borderphysics3.Mass = 0.5;
            testBoarder3.transform.position = new Vec2D(960, 750);
            scene.AddGameObject(testBoarder3);

            //Boarder right for test
            var testBoarder4 = new GameObject("BoarderRight");
            _ = testBoarder4.AddComponent<Boarder2>();
            var bordercollider4 = testBoarder4.AddComponent<BoxCollider>();
            bordercollider4.UpdateColliderPosition(new Vec2D(1970, 228));
            bordercollider4.UpdateColliderSize(5, 1070);
            var borderphysics4 = testBoarder4.AddComponent<PhysicsBody>();
            borderphysics4.IsMovable = false;
            borderphysics4.Mass = 0.5;
            testBoarder4.transform.position = new Vec2D(960, 750);
            scene.AddGameObject(testBoarder4);


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