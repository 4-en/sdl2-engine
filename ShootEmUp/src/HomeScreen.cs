using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp
{
    internal class HomeScreen
    {


        public static Scene CreateScene()
        {
            var scene = new Scene("Home Screen");
            Vec2D gameBounds = new Vec2D(1920, 1080);

            using (var activeScene = scene.Activate())
            {
                GameObject background = new GameObject("Background");
                background.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/space_background.jpg");
                background.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
                GameObject gameTitle = HomeScreenText("Shoot Em Up", gameBounds.x / 2, 300, 200);
                GameObject startText = HomeScreenText("Start", gameBounds.x / 2, 600, 130);
                GameObject shopText = HomeScreenText("Highscores", gameBounds.x / 2, 800, 130);
                scene.AddGameObject(background);
                scene.AddGameObject(gameTitle);
                scene.AddGameObject(startText);
                scene.AddGameObject(shopText);

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

                // Get the position and dimensions of the text object
                Vec2D position = gameObject.transform.position;
                Rect rect = new Rect(position.x - 200, position.y - 50, 400, 100);

                if (rect.Contains(mousePosition))
                {
                    if (gameObject.GetName().Equals("Start"))
                    {
                        LevelManager.StartNewRun();
                    }
                    if(gameObject.GetName().Equals("Highscores"))
                    {
                        // open url in browser
                        Console.WriteLine("Opening browser");
                        OpenUrl("http://api.aiko.lol:25500/view/spaceshooter");
                    }
                }

            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
