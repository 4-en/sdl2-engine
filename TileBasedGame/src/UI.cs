﻿using SDL2Engine;
using static SDL2.SDL;

namespace TileBasedGame
{
    public static class UI
    {

        public static Scene GameOverScene(int score, int time)
        {
            var scene = new Scene("GameOverScene");

            using (scene.Activate())
            {
                /*
                var background = new GameObject("GameOverBackground");
                var bgRenderer = background.AddComponent<FilledRect>();
                bgRenderer.SetRect(new Rect(0, 0, 1920, 1080));
                bgRenderer.color = new Color(0, 0, 0, 200);
                bgRenderer.anchorPoint = AnchorPoint.TopLeft;
                */


                var titleObject = new GameObject("GameOverTitle");
                titleObject.SetPosition(new Vec2D(0.5, 0.2));
                var titleRenderer = titleObject.AddComponent<TextRenderer>();
                titleRenderer.SetFontSize(94);
                titleRenderer.relativePosition = true;
                titleRenderer.SetText("Game Over");
                titleRenderer.anchorPoint = AnchorPoint.Center;

                var scoreObject = new GameObject("GameOverScore");
                scoreObject.SetPosition(new Vec2D(0.3, 0.4));
                var scoreRenderer = scoreObject.AddComponent<TextRenderer>();
                scoreRenderer.SetFontSize(48);
                scoreRenderer.relativePosition = true;
                scoreRenderer.SetText("Score: " + score);
                scoreRenderer.anchorPoint = AnchorPoint.Center;

                var timeObject = new GameObject("GameOverTime");
                timeObject.SetPosition(new Vec2D(0.7, 0.4));
                var timeRenderer = timeObject.AddComponent<TextRenderer>();
                timeRenderer.SetFontSize(48);
                timeRenderer.relativePosition = true;
                timeRenderer.SetText("Time Left: " + time);
                timeRenderer.anchorPoint = AnchorPoint.Center;

                var menuButtonTuple = Button("Main Menu", () => { LevelManager.LoadHomeScreen(); return true; }, new Rect(0, 0, 350, 150), Color.White, 440);
                var menuButton = menuButtonTuple.Item1;
                menuButton.SetPosition(new Vec2D(0.3, 0.7));
                menuButtonTuple.Item2.relativePosition = true;


                var tryAgainButtonTuple = Button("Try Again", () => { LevelManager.LoadLevel(LevelManager.levelIndex); return true; }, new Rect(0, 0, 350, 150), Color.White, 440);
                var tryAgainButton = tryAgainButtonTuple.Item1;
                tryAgainButton.SetPosition(new Vec2D(0.7, 0.7));
                tryAgainButtonTuple.Item2.relativePosition = true;
            }


            return scene;
        }

        public static Scene LevelCompletedScene(int score, int time, int currentLevel)
        {
            var scene = new Scene("GameOverScene");

            using (scene.Activate())
            {
                /*
                var background = new GameObject("GameOverBackground");
                var bgRenderer = background.AddComponent<FilledRect>();
                bgRenderer.SetRect(new Rect(0, 0, 1920, 1080));
                bgRenderer.color = new Color(0, 0, 0, 200);
                bgRenderer.anchorPoint = AnchorPoint.TopLeft;
                */


                var titleObject = new GameObject("GameOverTitle");
                titleObject.SetPosition(new Vec2D(0.5, 0.2));
                var titleRenderer = titleObject.AddComponent<TextRenderer>();
                titleRenderer.SetFontSize(94);
                titleRenderer.relativePosition = true;
                titleRenderer.SetText("Completed Level " + currentLevel);
                titleRenderer.anchorPoint = AnchorPoint.Center;

                var scoreObject = new GameObject("GameOverScore");
                scoreObject.SetPosition(new Vec2D(0.3, 0.4));
                var scoreRenderer = scoreObject.AddComponent<TextRenderer>();
                scoreRenderer.SetFontSize(48);
                scoreRenderer.relativePosition = true;
                scoreRenderer.SetText("Score: " + score);
                scoreRenderer.anchorPoint = AnchorPoint.Center;

                var timeObject = new GameObject("GameOverTime");
                timeObject.SetPosition(new Vec2D(0.7, 0.4));
                var timeRenderer = timeObject.AddComponent<TextRenderer>();
                timeRenderer.SetFontSize(48);
                timeRenderer.relativePosition = true;
                timeRenderer.SetText("Time Left: " + time);
                timeRenderer.anchorPoint = AnchorPoint.Center;

                var menuButtonTuple = Button("Main Menu", () => { LevelManager.LoadHomeScreen(); return true; }, new Rect(0, 0, 350, 150), Color.White, 440);
                var menuButton = menuButtonTuple.Item1;
                menuButton.SetPosition(new Vec2D(0.2, 0.7));
                menuButtonTuple.Item2.relativePosition = true;


                var tryAgainButtonTuple = Button("Try Again", () => { LevelManager.LoadLevel(LevelManager.levelIndex); return true; }, new Rect(0, 0, 350, 150), Color.White, 440);
                var tryAgainButton = tryAgainButtonTuple.Item1;
                tryAgainButton.SetPosition(new Vec2D(0.5, 0.7));
                tryAgainButtonTuple.Item2.relativePosition = true;

                var nextLevelButtonTuple = Button("Next Level", () => { LevelManager.LoadNextLevel(); return true; }, new Rect(0, 0, 350, 150), Color.White, 440);
                var nextLevelButton = nextLevelButtonTuple.Item1;
                nextLevelButton.SetPosition(new Vec2D(0.8, 0.7));
                nextLevelButtonTuple.Item2.relativePosition = true;

            }


            return scene;
        }
        public static GameObject EscapeMenu(string title = "Paused", Func<bool>? onContinue = null)
        {



            var menu = new GameObject("EscapeMenu");
            menu.SetPosition(new Vec2D(0.5, 0.3));
            menu.KeepInScene = true;

            var background = Component.CreateWithGameObject<FilledRect>("EscapeMenuBackground");
            background.Item2.SetRect(new Rect(0, 0, 4 * 1920, 4 * 1080));
            background.Item2.color = (new Color(0, 0, 0, 200));
            background.Item2.anchorPoint = AnchorPoint.TopLeft;
            menu.AddChild(background.Item1);
            background.Item1.SetPosition(new Vec2D(-2 * 1920, -2 * 1080));
            background.Item2.relativeToCamera = false;
            background.Item2.z_index = -100;

            var titleTextTuple = Component.CreateWithGameObject<TextRenderer>("EscapeMenuTitle");
            var titleText = titleTextTuple.Item2;
            titleTextTuple.Item1.SetPosition(new Vec2D(0, -0.2));
            titleText.SetFontSize(64);
            titleText.SetTextScale(0.1);
            titleText.SetText(title);
            titleText.SetPreferredSize(new Rect(0, 0, 50, 20));
            titleText.relativePosition = true;
            titleText.anchorPoint = AnchorPoint.Center;
            menu.AddChild(titleTextTuple.Item1);

            var button1Tuple = Button("Resume", () =>
            {
                menu.Destroy();
                onContinue?.Invoke();
                return true;
            }, new Rect(0, 0, 35, 15), Color.White, 44);

            var button1 = button1Tuple.Item1;
            button1.SetLocalPosition(new Vec2D(0, -0.05));
            button1Tuple.Item2.relativePosition = true;
            button1Tuple.Item2.z_index = -200;
            menu.AddChild(button1);

            var button2Tuple = Button("Main Menu", () => { LevelManager.LoadHomeScreen(); return true; }, new Rect(0, 0, 35, 15), Color.White, 44);
            var button2 = button2Tuple.Item1;
            button2.SetLocalPosition(new Vec2D(0, 0.150));
            button2Tuple.Item2.z_index = -200;
            button2Tuple.Item2.relativePosition = true;
            menu.AddChild(button2);

            var button3Tuple = Button("Quit", () => { Engine.Stop(); return true; }, new Rect(0, 0, 35, 15), Color.White, 44);
            var button3 = button3Tuple.Item1;
            button3Tuple.Item2.z_index = -200;
            button3.SetLocalPosition(new Vec2D(0, 0.350));
            button3Tuple.Item2.relativePosition = true;
            menu.AddChild(button3);

            menu.AddComponent<EscapeListener>().OnEscape = () =>
            {
                menu.Destroy();
                onContinue?.Invoke();
                return true;
            };

            return menu;
        }

        class EscapeListener : Script
        {

            public Func<bool>? OnEscape { get; set; }
            public override void Update()
            {
                if (Input.GetKeyDown(SDL_Keycode.SDLK_ESCAPE))
                {
                    if (OnEscape != null)
                    {
                        OnEscape.Invoke();
                    }
                }
            }
        }

        public static Tuple<GameObject, TextRenderer, TextRenderHelper> Button(
           string text,
           Func<bool> onClick,
           Rect? preferredSize = null,
           Color? color = null,
           int fontSize = 24)
        {
            var button = new GameObject(text + "_button");
            var textRenderer = button.AddComponent<TextRenderer>();
            var textRenderHelper = button.AddComponent<TextRenderHelper>();

            var prefSize = preferredSize ?? new Rect(0, 0, 250, 100);
            textRenderer.anchorPoint = AnchorPoint.Center;
            textRenderer.SetPreferredSize(prefSize);
            textRenderer.SetFontSize(fontSize);
            textRenderer.SetText(text);
            textRenderer.SetTextScale(0.1);
            textRenderer.color = color ?? Color.White;
            textRenderer.SetBorderSize(2);
            textRenderer.SetBorderColor(Color.White);

            textRenderHelper.OnHover += (object? source, TextRenderer renderer) =>
            {
                renderer.SetBackgroundColor(new Color(255, 50, 100, 100));
            };

            textRenderHelper.OnLeave += (object? source, TextRenderer renderer) =>
            {
                renderer.SetBackgroundColor(Color.Transparent);
            };

            textRenderHelper.OnClick += (object? source, TextRenderer renderer) =>
            {
                onClick.Invoke();
            };

            return new Tuple<GameObject, TextRenderer, TextRenderHelper>(button, textRenderer, textRenderHelper);

        }


    }

}