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
            var lp = leftPaddle.AddComponent<PhysicsBody>();
            lp.IsMovable = true;
            var leftPaddleBoxCollider = leftPaddle.AddComponent<BoxCollider>();
            leftPaddleBoxCollider.UpdateColliderPosition(new Vec2D(50, 750));
            leftPaddleBoxCollider.UpdateColliderSize(35, 160);
            leftPaddle.transform.position = new Vec2D(50, 750);
            scene.AddGameObject(leftPaddle);


            var rightPaddle = new GameObject("RightPaddle");
            _ = rightPaddle.AddComponent<ArrowKeysController>();
            _ = rightPaddle.AddComponent<Paddle>();
            var rp = rightPaddle.AddComponent<PhysicsBody>();
            rp.IsMovable = true;
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
            pb.IsMovable = true;

            //pongSquare.SetPosition(new Vec2D(960, 750));
            pongSquare.SetPosition(new Vec2D(960 - 480 - 50, 750));
            scene.AddGameObject(pongSquare);

            ////Boarder variante 1 
            //var leftBoarder = scene.CreateChild("Boarder");
            //_ = leftBoarder.AddComponent<Boarder>();
            //leftBoarder.transform.position = new Vec2D(5, 750);

            //var rightBoarder = scene.CreateChild("Boarder");
            //_ = rightBoarder.AddComponent<Boarder>();
            //rightBoarder.transform.position = new Vec2D(1915, 750);

            void CreateBoarder(string name, Vec2D position, Vec2D colliderPosition, Vec2D colliderSize)
            {
                var boarder = new GameObject(name);
                _ = boarder.AddComponent<Boarder2>();

                var collider = boarder.AddComponent<BoxCollider>();
                collider.UpdateColliderPosition(colliderPosition);
                collider.UpdateColliderSize((int)colliderSize.x, (int)colliderSize.y); // Hier die Umwandlung in int hinzugefügt

                var physics = boarder.AddComponent<PhysicsBody>();

                boarder.transform.position = position;

                scene.AddGameObject(boarder);
            }


            // Erstelle Boarder-Objekte mit vereinfachter Methode
            CreateBoarder("BoarderBottom", new Vec2D(960, 750), new Vec2D(0, 750 + 550), new Vec2D(1980, 5));
            CreateBoarder("BoarderTop", new Vec2D(960, 750), new Vec2D(0, 223), new Vec2D(1980, 5));
            CreateBoarder("BoarderLeft", new Vec2D(960, 750), new Vec2D(5, 228), new Vec2D(5, 1070));
            CreateBoarder("BoarderRight", new Vec2D(960, 750), new Vec2D(1970, 228), new Vec2D(5, 1070));



            return scene;
        }

        public static void Run()
        {

            var scene = CreateScene();

            var engine = new SDL2Engine.Engine(scene);

            engine.Run();
        }


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