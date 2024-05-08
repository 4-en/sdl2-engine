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

            // var leftPaddle = scene.CreateChild("Left Paddle");
            var leftPaddle = new GameObject("LeftPaddle");
            leftPaddle.AddComponent<WSController>();
            leftPaddle.AddComponent<Paddle>();

            var leftPaddleBoxCollider = leftPaddle.AddComponent<BoxCollider>();
            leftPaddleBoxCollider.UpdateColliderSize(35, 160);
            leftPaddle.transform.position = new Vec2D(50, 0);
            scene.AddGameObject(leftPaddle);


            var rightPaddle = new GameObject("RightPaddle");
            rightPaddle.AddComponent<ArrowKeysController>();
            rightPaddle.AddComponent<Paddle>();

            var rightPaddleBoxCollider = rightPaddle.AddComponent<BoxCollider>();
            rightPaddleBoxCollider.UpdateColliderSize(35, 160);
            rightPaddle.transform.position = new Vec2D(1870, borderPosition.y);

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
            var bc = pongSquare.AddComponent<BoxCollider>();
            bc.box = new Rect(-25, -25, 50, 50);
            var pb = pongSquare.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(800, 500);
            pb.IsMovable = true;

            pongSquare.SetPosition(new Vec2D(960, borderPosition.y));
            //pongSquare.SetPosition(new Vec2D(960 - 480 - 50, 750));
            scene.AddGameObject(pongSquare);

            ////Boarder variante 1 
            //var leftBoarder = scene.CreateChild("Boarder");
            //_ = leftBoarder.AddComponent<Boarder>();
            //leftBoarder.transform.position = new Vec2D(5, 750);

            //var rightBoarder = scene.CreateChild("Boarder");
            //_ = rightBoarder.AddComponent<Boarder>();
            //rightBoarder.transform.position = new Vec2D(1915, 750);

            /*
            void CreateBorder(string name, Vec2D position, Vec2D colliderPosition, Vec2D colliderSize)
            {
                var boarder = new GameObject(name);
                var boarder2 = boarder.AddComponent<Boarder2>();

                var collider = boarder.AddComponent<BoxCollider>();
                collider.UpdateColliderPosition(colliderPosition);
                collider.UpdateColliderSize((int)colliderSize.x, (int)colliderSize.y); // Hier die Umwandlung in int hinzugefügt

                var physics = boarder.AddComponent<PhysicsBody>();

                boarder.transform.position = position;

                scene.AddGameObject(boarder);
            }

            //public static float BoarderWidth = 1905;
            //public static float BoarderHeight = 850;

            // Erstelle Boarder-Objekte mit vereinfachter Methode
            CreateBorder("BoarderBottom", boarderPosition, new Vec2D(5, boarderPosition.y + Boarder2.BoarderHeight / 2 + 20), new Vec2D(Boarder2.BoarderWidth, 5));
            CreateBorder("BoarderTop", boarderPosition, new Vec2D(5, boarderPosition.y - Boarder2.BoarderHeight / 2 + 20), new Vec2D(Boarder2.BoarderWidth - 5, 5));
            CreateBorder("BoarderLeft", boarderPosition, new Vec2D(0, boarderPosition.y - Boarder2.BoarderHeight / 2), new Vec2D(5, Boarder2.BoarderHeight));
            CreateBorder("BoarderRight", boarderPosition, new Vec2D(Boarder2.BoarderWidth + 25, boarderPosition.y - Boarder2.BoarderHeight / 2), new Vec2D(5, Boarder2.BoarderHeight));
            */

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