using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.src
{
    internal class HomeScreen
    {


        public static Scene CreateScene()
        {
            var scene = new Scene("Home Screen");
            Vec2D gameBounds = new Vec2D(1920, 1080);

            using (var activeScene = scene.Activate())
            {
                GameObject gameTitle = HomeScreenText("Shoot Em Up", gameBounds.x / 2, 300, 200);
                GameObject startText = HomeScreenText("Start", gameBounds.x / 2, 600, 130);
                scene.AddGameObject(gameTitle);
                scene.AddGameObject(startText);

                
            }

            return scene;
        }


        internal static GameObject HomeScreenText(string text, double x, double y, int fontSize)
        {

            var textObject = new GameObject(text);
            textObject.transform.position = new Vec2D(x, y);
            var textComponent = textObject.AddComponent<TextRenderer>();
            textComponent.color = new Color(65, 105, 255, 255);
            textComponent.SetFontSize(fontSize);
            textComponent.SetText(text);
            textComponent.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            textObject.AddComponent<MenuMouseTracker>();
            var helper = textObject.AddComponent<TextRenderHelper>();
            helper.OnHover += (object? source, TextRenderer renderer) =>
            {
                renderer.SetColor(new Color(0, 0, 255,255));
                renderer.SetFontSize(renderer.GetText() != "Shoot Em Up" ? 150 : 200);


            };

            helper.OnLeave += (object? source, TextRenderer renderer) =>
            {
                renderer.SetColor(new Color(65, 105, 255, 255));
                renderer.SetFontSize(renderer.GetText() != "Shoot Em Up" ? 130 : 200);


            };

            return textObject;

        }
    }




    class MenuMouseTracker : Script
    {
        public event Action<string> OnClick;

        public override void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vec2D mousePosition = Input.GetMousePosition();
                GameObject gameObject = this.gameObject;
                Camera camera = Camera.GetCamera(gameObject);

                if (camera != null)
                {
                    mousePosition = camera.ScreenToWorld(mousePosition);
                }

                TextRenderer textRenderer = gameObject.GetComponent<TextRenderer>();

                // Get the position and dimensions of the text object
                Vec2D position = gameObject.transform.position;
                Rect rect = new Rect(position.x - 200, position.y - 50, 400, 100);

                if (rect.Contains(mousePosition))
                {
                    if (gameObject.GetName().Equals("Start"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel1());
                    }
                }

            }
        }
    }
}
