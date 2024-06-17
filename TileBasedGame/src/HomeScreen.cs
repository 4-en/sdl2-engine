using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TileBasedGame.src
{
    internal class HomeScreen
    {

        public static String gameName = "Tile Based Game";

        public static Scene CreateScene()
        {
            var scene = new Scene("Home Screen");
            Vec2D gameBounds = new Vec2D(1920, 1080);

            using (var activeScene = scene.Activate())
            {
                GameObject background = new GameObject("Background");
                background.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/space_background.jpg");
                background.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
                GameObject gameTitle = HomeScreenText(HomeScreen.gameName, gameBounds.x / 2, 200, 200);
                GameObject level1Text = HomeScreenText("Level 1", gameBounds.x / 2, 450, 100);
                GameObject level2Text = HomeScreenText("Level 2", gameBounds.x / 2, 550, 100);
                GameObject level3Text = HomeScreenText("Level 3", gameBounds.x / 2, 650, 100);
                GameObject level4Text = HomeScreenText("Level 4", gameBounds.x / 2, 750, 100);
                GameObject level5Text = HomeScreenText("Level 5", gameBounds.x / 2, 850, 100);
                scene.AddGameObject(background);
                scene.AddGameObject(gameTitle);
                scene.AddGameObject(level1Text);
                scene.AddGameObject(level2Text);
                scene.AddGameObject(level3Text);
                scene.AddGameObject(level4Text);
                scene.AddGameObject(level5Text);


                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;
                player.playOnAwake = true;
                player.SetSource("Assets/Audio/home.mp3");



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
                renderer.SetColor(new Color(0, 0, 255, 255));
                renderer.SetFontSize(renderer.GetText() != HomeScreen.gameName ? 125 : 200);


            };

            helper.OnLeave += (object? source, TextRenderer renderer) =>
            {
                renderer.SetColor(new Color(65, 105, 255, 255));
                renderer.SetFontSize(renderer.GetText() != HomeScreen.gameName ? 100 : 200);


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

                // Get the position and dimensions of the text object
                Vec2D position = gameObject.transform.position;
                Rect rect = new Rect(position.x - 200, position.y - 50, 400, 100);

                if (rect.Contains(mousePosition))
                {
                    if (gameObject.GetName().Equals("Start"))
                    {
                        LevelManager.StartNewRun();
                    }
                    if (gameObject.GetName().Equals("Level 1"))
                    {
                        LevelManager.levelIndex = 0;
                        LevelManager.LoadNextLevel();
                    }
                    if (gameObject.GetName().Equals("Level 2"))
                    {
                        LevelManager.levelIndex = 1;
                        LevelManager.LoadNextLevel();
                    }
                    if (gameObject.GetName().Equals("Level 3"))
                    {
                        LevelManager.levelIndex = 2;
                        LevelManager.LoadNextLevel();
                    }
                    if (gameObject.GetName().Equals("Level 4"))
                    {
                        LevelManager.levelIndex = 3;
                        LevelManager.LoadNextLevel();
                    }
                    if (gameObject.GetName().Equals("Level 5"))
                    {
                        LevelManager.levelIndex = 4;
                        LevelManager.LoadNextLevel();
                    }
                }

            }
        }
    }
}
