using SDL2Engine.Utils;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ShootEmUp.src
{
   
        public class GameController : Script
        {
            public int level_id = 0;

            private GameObject? escapeMenu = null;

            protected Vec2D gameBounds = new Vec2D(1920, 1080);

            protected double roundTimer = -3;
            public double gameTimer = 0;

            protected TextRenderer? timeText = null;
            private bool stopped = false;


            public bool IsStopped()
            {
                return stopped;
            }
            public override void Start()
            {
                // create basic game object here
                // (anything that needs to have a reference to it)

                gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);

                //Time Counter
                timeText = Component.CreateWithGameObject<TextRenderer>("TimeCounter").Item2;
                timeText.color = new Color(255, 255, 255, 205);
                timeText.SetFontSize(52);
                timeText.anchorPoint = AnchorPoint.TopLeft;
                timeText.GetGameObject().SetPosition(new Vec2D(gameBounds.x - 300, 25));


                // create the player
                var player = new GameObject("Player");
                var texture = player.AddComponent<TextureRenderer>();
                texture?.SetSource("Assets/Textures/change_direction_powerup.png");
                BoxCollider.FromDrawableRect(player);
                player.AddComponent<KeyboardController>();
                player.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y/2);
                var pb = player.AddComponent<PhysicsBody>();
                pb.Velocity = new Vec2D(100, 0);


        }

          


            


       

            public override void Update()
            {

                if (timeText != null)
                {
                    // 1 decimal place
                    timeText.SetText($"Time: {Math.Round(roundTimer, 1)}");
                }

                if (Input.GetKeyDown((int)SDL_Keycode.SDLK_ESCAPE))
                {
                    if (this.escapeMenu != null)
                    {
                        this.escapeMenu.Destroy();
                        this.escapeMenu = null;
                    }
                    else
                    {
                        var stopState = !!stopped;
                        var escapemenu = UI.EscapeMenu("Paused", () =>
                        {

                            this.stopped = stopState;
                            GetScene()?.SetPhysics(true);
                            this.escapeMenu = null;
                            return true;
                        });

                        this.escapeMenu = escapemenu;
                        this.stopped = true;
                        GetScene()?.SetPhysics(false);
                    }
                }




            // basic game logic here
            if (stopped) return;


                gameTimer += Time.deltaTime;
                roundTimer += Time.deltaTime;

               

              
              
            }
        }
    


}

public class KeyboardController : Script
{
    public int left = (int)SDL_Keycode.SDLK_a;
    public int right = (int)SDL_Keycode.SDLK_d;

    public override void Start()
    {
        
    }

    public override void Update()
    {
       

        if (Input.GetKeyPressed(left))
        {
            gameObject.transform.rotation -= 0.5;
        }
        if (Input.GetKeyPressed(right))
        {
            gameObject.transform.rotation += 0.5;
        }
        //rotate velocity
        gameObject.GetComponent<PhysicsBody>().Velocity = new Vec2D(100, 0).Rotate(gameObject.transform.rotation);
    }
}