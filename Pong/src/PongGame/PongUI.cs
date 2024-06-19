using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using static SDL2.SDL;
using Pong.src;

namespace Pong
{
    public static class UI
    {

        private static PlayerType StringToPlayerType(string s)
        {
            switch (s)
            {
                case "W/S":
                    return PlayerType.WS;
                case "Arrows":
                    return PlayerType.ArrowKeys;
                case "AI":
                    return PlayerType.AI;
                case "Mouse":
                    return PlayerType.Mouse;
                case "Controller":
                    return PlayerType.Controller;
                case "UNFAIR":
                    return PlayerType.UNFAIR;
                default:
                    return PlayerType.WS;
            }
        }

        private static GameMode StringToMode(string s)
        {
            switch (s)
            {
                case "Duel":
                    return GameMode.DUEL;
                case "Endless":
                case "Highscore":
                case "HiScore":
                case "Hiscore":
                    return GameMode.HIGHSCORE;
                case "Timed":
                    return GameMode.TIMED;
                default:
                    return GameMode.DUEL;
            }
        }

        public static Scene PlayerSelectScene()
        {
            var scene = new Scene("PlayerSelectScene");

            using (var _ = scene.Activate())
            {

                Rect gameBounds = new Rect(0, 0, 1920, 1080);
                
                var background = new GameObject("Background");
                /*
                var bg_renderer = background.AddComponent<FilledRect>();
                bg_renderer.color = Color.Cyan;
                bg_renderer.SetRect(gameBounds);
                bg_renderer.anchorPoint = AnchorPoint.TopLeft;
                */
                

                var text_root = background.CreateChild("TextRoot");
                text_root.SetPosition(new Vec2D(1920 / 2, 100));

                GameObject gameTitle = HomeScreen.HomeScreenText("Pong", 0, 0, 200);
                text_root.AddChild(gameTitle);

                var player1 = text_root.CreateChild("Player1");
                player1.SetLocalPosition(new Vec2D(-100, 200));
                var player1_renderer = player1.AddComponent<TextRenderer>();
                player1_renderer.anchorPoint = AnchorPoint.TopRight;
                player1_renderer.SetText("Player 1");
                player1_renderer.SetFontSize(48);
                player1_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

                string[] control_values = new string[] { "W/S", "Arrows", "Mouse", "AI", "UNFAIR" };


                var player1_controls = UI.CycleButton(
                    control_values,
                    (string s) =>
                    {
                        LevelManager.player1Type = StringToPlayerType(s);
                        return true;
                    },
                    new Rect(0, 0, 250, 100),
                    Color.White,
                    48);

                var PlayerTypeToString = new Dictionary<PlayerType, string>
                {
                    { PlayerType.WS, "W/S" },
                    { PlayerType.ArrowKeys, "Arrows" },
                    { PlayerType.AI, "AI" },
                    { PlayerType.Mouse, "Mouse" },
                    { PlayerType.Controller, "Controller" },
                    { PlayerType.UNFAIR, "UNFAIR" }
                };

                player1_controls.Item2.SetText(PlayerTypeToString[LevelManager.player1Type]);

                var player1_controls_obj = player1_controls.Item1;
                player1_controls_obj.SetLocalPosition(new Vec2D(300, 0));
                player1.AddChild(player1_controls_obj);

                var player2 = text_root.CreateChild("Player2");
                player2.SetLocalPosition(new Vec2D(-100, 400));
                var player2_renderer = player2.AddComponent<TextRenderer>();
                player2_renderer.anchorPoint = AnchorPoint.TopRight;
                player2_renderer.SetText("Player 2");
                player2_renderer.SetFontSize(48);
                player2_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

                var player2_controls = UI.CycleButton(
                    control_values,
                    (string s) =>
                    {
                        LevelManager.player2Type = StringToPlayerType(s);
                        return true;
                    },
                    new Rect(0, 0, 250, 100),
                    Color.White,
                    48);

                player2_controls.Item2.SetText(PlayerTypeToString[LevelManager.player2Type]);
                var player2_controls_obj = player2_controls.Item1;
                player2_controls_obj.SetLocalPosition(new Vec2D(300, 0));
                player2.AddChild(player2_controls_obj);

                string[] mode_values = new string[] { "Duel", "Hiscore", "Timed" };

                var mode = text_root.CreateChild("Mode");
                mode.SetLocalPosition(new Vec2D(-100, 600));
                var mode_renderer = mode.AddComponent<TextRenderer>();
                mode_renderer.anchorPoint = AnchorPoint.TopRight;
                mode_renderer.SetText("Mode");
                mode_renderer.SetFontSize(48);
                mode_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
                
                var mode_button = UI.CycleButton(
                    mode_values,
                    (string s) =>
                    {
                        LevelManager.gameMode = StringToMode(s);
                        return true;
                    },
                    new Rect(0, 0, 250, 100),
                    Color.White,
                    48);

                var mode_button_obj = mode_button.Item1;
                mode_button_obj.SetLocalPosition(new Vec2D(300, 0));
                mode.AddChild(mode_button_obj);
                

                var level_select_button = UI.Button(
                    "Select Level",
                    () =>
                    {
                        LevelManager.LoadHomeScreen();
                        return true;
                    },
                    new Rect(0, 0, 300, 100),
                    Color.White,
                    48);

                var level_select_button_obj = level_select_button.Item1;
                level_select_button_obj.SetLocalPosition(new Vec2D(0, 800));
                text_root.AddChild(level_select_button_obj);

            }
            return scene;
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

        public static GameObject EscapeMenu(string title = "Paused", Func<bool>? onContinue = null)
        {



            var menu = new GameObject("EscapeMenu");
            menu.SetPosition(new Vec2D(1920 / 2, 400));

            var background = Component.CreateWithGameObject<FilledRect>("EscapeMenuBackground");
            background.Item2.SetRect(new Rect(0, 0, 1920, 1080));
            background.Item2.color = (new Color(0, 0, 0, 200));
            background.Item2.anchorPoint = AnchorPoint.TopLeft;
            menu.AddChild(background.Item1);
            background.Item1.SetPosition(new Vec2D(0, 0));

            var titleTextTuple = Component.CreateWithGameObject<TextRenderer>("EscapeMenuTitle");
            var titleText = titleTextTuple.Item2;
            titleTextTuple.Item1.SetPosition(new Vec2D(0, -200));
            titleText.SetFontSize(64);
            titleText.SetText(title);
            titleText.SetPreferredSize(new Rect(0, 0, 500, 200));
            titleText.anchorPoint = AnchorPoint.Center;
            menu.AddChild(titleTextTuple.Item1);

            var button1Tuple = Button("Resume", () =>
            {
                menu.Destroy();
                onContinue?.Invoke();
                return true;
            }, new Rect(0, 0, 350, 150), Color.White, 44);

            var button1 = button1Tuple.Item1;
            button1.SetLocalPosition(new Vec2D(0, -50));
            menu.AddChild(button1);

            var button2Tuple = Button("Main Menu", () => { LevelManager.LoadPlayerSelection(); return true; }, new Rect(0, 0, 350, 150), Color.White, 44);
            var button2 = button2Tuple.Item1;
            button2.SetLocalPosition(new Vec2D(0, 150));
            menu.AddChild(button2);

            var button3Tuple = Button("Quit", () => { Engine.Stop(); return true; }, new Rect(0, 0, 350, 150), Color.White, 44);
            var button3 = button3Tuple.Item1;
            button3.SetLocalPosition(new Vec2D(0, 350));
            menu.AddChild(button3);

            menu.AddComponent<EscapeListener>().OnEscape = () =>
            {
                menu.Destroy();
                onContinue?.Invoke();
                return true;
            };

            return menu;
        }

        public static void ArrangeChildren(GameObject parent, bool horizontal = true, int padding = 10)
        {
            var children = parent.GetChildren();
            if (children.Count == 0)
            {
                return;
            }
            if (horizontal)
            {
                double totalWidth = 0;
                foreach (var child in children)
                {
                    totalWidth += child.GetComponent<DrawableRect>()?.GetRect().w ?? 0;
                }

                totalWidth += (children.Count - 1) * padding;

                double x_start = -totalWidth / 2;
                foreach (var child in children)
                {
                    var rect = child.GetComponent<DrawableRect>()?.GetRect();
                    if (rect != null)
                    {
                        child.SetLocalPosition(new Vec2D(x_start + rect.Value.w / 2, 0));
                        x_start += rect.Value.w + padding;
                    }
                }
            }
            else
            {
                double totalHeight = 0;
                foreach (var child in children)
                {
                    totalHeight += child.GetComponent<DrawableRect>()?.GetRect().h ?? 0;
                }

                totalHeight += (children.Count - 1) * padding;

                double y_start = -totalHeight / 2;
                foreach (var child in children)
                {
                    var rect = child.GetComponent<DrawableRect>()?.GetRect();
                    if (rect != null)
                    {
                        child.SetLocalPosition(new Vec2D(0, y_start + rect.Value.h / 2));
                        y_start += rect.Value.h + padding;
                    }
                }
            }

        }

        public static Tuple<GameObject, TextRenderer, TextRenderHelper> CycleButton(
            string[] values,
            Func<string, bool> onChange,
            Rect? preferredSize = null,
            Color? color = null,
            int fontSize = 24)
        {

            var tuple = Button(
                values[0],
                () =>
                {
                    return true;
                },
                preferredSize,
                color,
                fontSize
             );

            var button = tuple.Item1;
            var textRenderer = tuple.Item2;
            var textRenderHelper = tuple.Item3;

            string[] strings = new string[values.Length];
            values.CopyTo(strings, 0);

            textRenderHelper.OnClick += (object? source, TextRenderer renderer) =>
            {
                var index = Array.IndexOf(strings, renderer.GetText());
                index = (index + 1) % strings.Length;
                renderer.SetText(strings[index]);
                onChange.Invoke(strings[index]);
            };


            return tuple;

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
