using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TileBasedGame
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
                GameObject gameTitle = HomeScreenText(HomeScreen.gameName, 0.5, 0.2, 200);
                var gameTitleRenderer = gameTitle.GetComponent<TextRenderer>();
                GameObject level1Text = HomeScreenText("Level 1", 0.5, 0.4, 100);
                GameObject level2Text = HomeScreenText("Level 2", 0.5, 0.5, 100);
                GameObject level3Text = HomeScreenText("Level 3", 0.5, 0.6, 100);
                GameObject level4Text = HomeScreenText("Level 4", 0.5, 0.7, 100);
                GameObject level5Text = HomeScreenText("Level 5", 0.5, 0.8, 100);
            }

            return scene;
        }


        internal static GameObject HomeScreenText(string text, double x, double y, int fontSize)
        {

            var textObject = new GameObject(text);
            textObject.transform.position = new Vec2D(x, y);
            var textComponent = textObject.AddComponent<TextRenderer>();
            textComponent.relativePosition = true;
            textComponent.color = new Color(65, 105, 255, 255);
            textComponent.SetFontSize(fontSize);
            textComponent.SetText(text);
            textComponent.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
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

            helper.OnClick += (object? source, TextRenderer renderer) =>
            {
                if (textObject.GetName().Equals("Start"))
                {
                    LevelManager.StartNewRun();
                }
                if (textObject.GetName().Equals("Level 1"))
                {
                    LevelManager.levelIndex = 0;
                    LevelManager.LoadNextLevel();
                }
                if (textObject.GetName().Equals("Level 2"))
                {
                    LevelManager.levelIndex = 1;
                    LevelManager.LoadNextLevel();
                }
                if (textObject.GetName().Equals("Level 3"))
                {
                    LevelManager.levelIndex = 2;
                    LevelManager.LoadNextLevel();
                }
                if (textObject.GetName().Equals("Level 4"))
                {
                    LevelManager.levelIndex = 3;
                    LevelManager.LoadNextLevel();
                }
                if (textObject.GetName().Equals("Level 5"))
                {
                    LevelManager.levelIndex = 4;
                    LevelManager.LoadNextLevel();
                }
            };

            return textObject;

        }
    }
}
