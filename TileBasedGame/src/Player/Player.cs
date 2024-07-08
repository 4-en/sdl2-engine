using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace TileBasedGame
{

    public class Player : Entities.Entity
    {
        private DrawableRect? renderer;

        public static Player CreatePlayer()
        {
            var player = new GameObject("Player");
            var playerComp = player.AddComponent<Player>();

            return playerComp;
        }
        public override void Start()
        {
            base.Start();

            renderer = AddComponent<FilledRect>();
            renderer.color = new Color(155, 30, 200, 255);
            renderer.SetRect(new Rect(0, 0, 10, 20));
            renderer.anchorPoint = AnchorPoint.BottomCenter;
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
            AddComponent<CameraController>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;

        }

        public override void Update()
        {
            base.Update();
            if(physicsBody == null)
            {
                return;
            }
            bool isShooting = false;
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_1))
            {
                isShooting = true;
            }
            bool movementKeyPressed = false;
            if(Input.GetKeyDown(SDL_Keycode.SDLK_SPACE))
            {
                Jump();
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_a))
            {
                MoveLeft();
                movementKeyPressed = true;
                if(!isShooting)
                    facingRight = false;
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                MoveRight();
                movementKeyPressed = true;
                if(!isShooting)
                    facingRight = true;
            }

            if (!movementKeyPressed)
            {
                Decellerate();
            }
        }
    }
}