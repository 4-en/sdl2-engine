﻿using SDL2;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static System.Formats.Asn1.AsnWriter;

namespace Pong.src
{
    internal class HomeScreen
    {


        public static Scene CreateScene()
        {
            var scene = new Scene("Home Screen");
            Vec2D gameBounds = new Vec2D(1920, 1080);

            using (var activeScene = scene.Activate())
            {
                GameObject gameTitle = HomeScreenText("Pong", gameBounds.x / 2, 200, 200);
                GameObject level1 = HomeScreenText("Level 1", gameBounds.x / 2, 500, 100);
                GameObject level2 = HomeScreenText("Level 2", gameBounds.x / 2, 600, 100);
                GameObject level3 = HomeScreenText("Level 3", gameBounds.x / 2, 700, 100);
                GameObject level4 = HomeScreenText("Level 4", gameBounds.x / 2, 800, 100);
                GameObject level5 = HomeScreenText("Level 5", gameBounds.x / 2, 900, 100);

                // to player select button in top left
                var toSelectTuple = UI.Button("Back", () =>
                {
                    LevelManager.LoadPlayerSelection();
                    return true;
                });

                toSelectTuple.Item1.transform.position = new Vec2D(170, 100);
                toSelectTuple.Item2.anchorPoint = AnchorPoint.Center;
                toSelectTuple.Item2.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
                toSelectTuple.Item2.SetFontSize(50);
            }

            return scene;
        }


        internal static GameObject HomeScreenText(string text, double x, double y, int fontSize)
        {

            var textObject = new GameObject(text);
            textObject.transform.position = new Vec2D(x, y);
            var textComponent = textObject.AddComponent<TextRenderer>();
            textComponent.color = new Color(148, 0, 211, 255);
            textComponent.SetFontSize(fontSize);
            textComponent.SetText(text);
            textComponent.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            textObject.AddComponent<MenuMouseTracker>();
            var helper = textObject.AddComponent<TextRenderHelper>();
            helper.OnHover += (object? source, TextRenderer renderer) =>
            {
                renderer.SetColor(Color.Gold);
                renderer.SetFontSize(renderer.GetText() != "Pong" ? 110 : 200);


            };

            helper.OnLeave += (object? source, TextRenderer renderer) =>
            {
                renderer.SetColor(new Color(148, 0, 211, 255));
                renderer.SetFontSize(renderer.GetText() != "Pong" ? 100 : 200);


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
                    if (gameObject.GetName().Equals("Level 1"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel1());
                    }
                    if (gameObject.GetName().Equals("Level 2"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel2());
                    }
                    if (gameObject.GetName().Equals("Level 3"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel3());
                    }
                    if (gameObject.GetName().Equals("Level 4"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel4());
                    }
                    if (gameObject.GetName().Equals("Level 5"))
                    {
                        SceneManager.SetScene(LevelManager.CreateLevel5());
                    }
                }

            }
        }
    }
}