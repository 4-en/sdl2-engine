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
    internal class HomeScreen
    {
        public static Scene CreateScene()
        {
            var scene = new Scene("Home Screen");

            var homeScreenScript = new GameObject("TestScript");
            homeScreenScript.AddComponent<HomeScreenScript>();
            scene.AddGameObject(homeScreenScript);

            return scene;
        }
    }


    public class HomeScreenScript : Script
    {
        

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        public override void Start()
        {
           

            gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);

            

            var gameTitle = new GameObject("gameTitle");
            gameTitle.transform.position = new Vec2D(gameBounds.x / 2, 50);
            var titleText = gameTitle.AddComponent<TextRenderer>();
            titleText.color = new Color(0, 255, 255, 205);
            titleText.SetFontSize(100);
            titleText.SetText("Pong");
            //titleText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            titleText.SetFontPath("Assets/Fonts/Roboto-Regular.ttf");


        }
    }
}
