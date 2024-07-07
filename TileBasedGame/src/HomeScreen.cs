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
                var bg_renderer = background.AddComponent<BackgroundRenderer>();
                bg_renderer.SetSource("Assets/Textures/space_background.jpg");
                bg_renderer.SetRect(new Rect(0, 0, 192, 108));
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

        internal static GameObject LockObject(double x, double y)
        {
            
            GameObject newLock = new GameObject("Lock");
            newLock.transform.position = new Vec2D(x,y-0.01);
            var lockOne = newLock.AddComponent<SpriteRenderer>();
            lockOne.anchorPoint = AnchorPoint.Center;
            lockOne.SetSource("Assets/Textures/lock.png");
            lockOne.SetWorldSize(80, 80);
            lockOne.relativePosition = true;
            lockOne.SetZIndex(-1);
            return new GameObject("Lock");
        }


        internal static GameObject HomeScreenText(string text, double x, double y, int fontSize)
        {
            var textObject = new GameObject(text);
            //add a lock for every level which is not unlocked
            var unlockedLevel = LevelManager.unlockedLevel;
            if (text.Equals("Level 2") && unlockedLevel < 2)
            {
                textObject.AddChild(LockObject(x, y));
            }
            if (text.Equals("Level 3") && unlockedLevel < 3)
            {
                textObject.AddChild(LockObject(x, y));
            }
            if (text.Equals("Level 4") && unlockedLevel < 4)
            {
                textObject.AddChild(LockObject(x, y));
            }
            if (text.Equals("Level 5") && unlockedLevel < 5)
            {
                textObject.AddChild(LockObject(x, y));
            }

            textObject.transform.position = new Vec2D(x, y);
            var textComponent = textObject.AddComponent<TextRenderer>();
            textComponent.relativePosition = true;
            textComponent.color = new Color(65, 105, 255, 255);
            textComponent.SetFontSize(fontSize);
            textComponent.SetText(text);
            textComponent.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            textComponent.SetZIndex(0);
            var helper = textObject.AddComponent<TextRenderHelper>();
            helper.OnHover += (object? source, TextRenderer renderer) =>
            {
                if (textObject.GetChild("Lock") == null)
                {
                    renderer.SetFontSize(renderer.GetText() != HomeScreen.gameName ? 125 : 200);
                }


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
                    if (unlockedLevel > 1)
                    {
                        LevelManager.levelIndex = 1;
                        LevelManager.LoadNextLevel();
                    }
                }
                if (textObject.GetName().Equals("Level 3"))
                {
                    if (unlockedLevel > 2)
                    {
                        LevelManager.levelIndex = 2;
                        LevelManager.LoadNextLevel();
                    }
                }
                if (textObject.GetName().Equals("Level 4"))
                {
                    if (unlockedLevel > 3)
                    {
                        LevelManager.levelIndex = 3;
                        LevelManager.LoadNextLevel();
                    }
                }
                if (textObject.GetName().Equals("Level 5"))
                {
                    if (unlockedLevel > 4)
                    {
                        LevelManager.levelIndex = 4;
                        LevelManager.LoadNextLevel();
                    }
                }
            };

            return textObject;

        }
    }
}
