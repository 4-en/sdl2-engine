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

        public static Vec2D borderPosition = new Vec2D(960, 500);

        public static Scene CreateScene()
        {

            var scene = new Scene("PongGame Scene");

            ////PongBall variante round
            var pongSquare = new GameObject("PongSquare");
            //_ = pongBall.AddComponent<ArrowKeysController>();
            var ball_drawable = pongSquare.AddComponent<FilledRect>();
            ball_drawable.color = new Color(255, 255, 255, 255);
            ball_drawable.SetRect(new Rect(50, 50));
            ball_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(pongSquare);
            var pb = pongSquare.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(80, 50);
            pb.IsMovable = true;

            pongSquare.SetPosition(new Vec2D(960, borderPosition.y));
            //pongSquare.SetPosition(new Vec2D(960 - 480 - 50, 750));
            scene.AddGameObject(pongSquare);

            // var leftPaddle = scene.CreateChild("Left Paddle");
            var leftPaddle = new GameObject("LeftPaddle", scene);
            leftPaddle.AddComponent<WSController>();
            var left_paddle_drawable = leftPaddle.AddComponent<DrawableRect>();
            left_paddle_drawable.color = new Color(055, 055, 255, 255);
            left_paddle_drawable.SetRect(new Rect(35, 160));
            left_paddle_drawable.anchorPoint = AnchorPoint.TopLeft;


            BoxCollider.FromDrawableRect(leftPaddle);
            //leftPaddleBoxCollider.box = new Rect(0, 0, 35, 160);
            leftPaddle.transform.position = new Vec2D(50, 1080/2 - 80);


            var rightPaddle = new GameObject("RightPaddle");
            rightPaddle.AddComponent<ArrowKeysController>();
            var right_paddle_drawable = rightPaddle.AddComponent<DrawableRect>();
            right_paddle_drawable.color = new Color(255, 055, 055, 255);
            right_paddle_drawable.SetRect(new Rect(35, 160));
            right_paddle_drawable.anchorPoint = AnchorPoint.TopLeft;

            BoxCollider.FromDrawableRect(rightPaddle);
            rightPaddle.transform.position = new Vec2D(1870, borderPosition.y);

            scene.AddGameObject(rightPaddle);

            var create_barrier = (string name, Rect area) =>
            {
                var barrier = new GameObject(name, scene);
                var collider = barrier.AddComponent<BoxCollider>();
                collider.box = area;
            };

            create_barrier("Top Barrier", new Rect(0, -100, 1920, 100));
            create_barrier("Bottom Barrier", new Rect(0, 1080, 1920, 100));

            // we don't really need the right and left barriers. We can just use a Script to check if the ball is out of bounds
            create_barrier("Left Barrier", new Rect(-100, 0, 100, 1080));
            create_barrier("Right Barrier", new Rect(1920, 0, 100, 1080));





            return scene;
        }

        public static void Run()
        {

            var scene = CreateScene();

            var engine = new SDL2Engine.Engine(scene);

            engine.Run();
        }

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