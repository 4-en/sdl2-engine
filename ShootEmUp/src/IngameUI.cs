using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ShootEmUp
{
    internal class IngameUI
    {
        internal class UserInterface : Script
        {

            public override void Start()
            {
                AddComponent<HealthBar>();
                AddComponent<HighScore>();
                //AddComponent<MoneyIndicator>();
                AddComponent<HealthReducer>();
                AddComponent<HighscoreUpdater>();
                AddComponent<MoneyUpdater>();
                AddComponent<WaveIndicator>();
                AddComponent<ShieldTexture>();

            }
        }

        internal class HealthReducer : Script
        {
            public override void Update()
            {
                //key inputs
                if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_1))
                {
                    if (Player.currentHealth > 0)
                    {

                        Player.currentHealth -= 1;
                    }
                }
                if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_2))
                {
                    if (Player.currentHealth < Player.maxHealth)
                    {
                        Player.currentHealth += 1;
                    }
                }

            }
        }

        internal class HighscoreUpdater : Script
        {
            public override void Update()
            {
                //key inputs
                if (Input.GetKeyDown((int)SDL_Keycode.SDLK_3))
                {
                    PlayerData.Instance.TotalScore += 100;
                }
                if (Input.GetKeyDown((int)SDL_Keycode.SDLK_4))
                {
                    PlayerData.Instance.TotalScore -= 100;
                }
            }
        }

        internal class MoneyUpdater : Script
        {
            public override void Update()
            {
                //key inputs
                if (Input.GetKeyDown((int)SDL_Keycode.SDLK_5))
                {
                    PlayerData.Instance.Money += 100;
                }
                if (Input.GetKeyDown((int)SDL_Keycode.SDLK_6))
                {
                    PlayerData.Instance.Money -= 100;
                }
            }
        }

        internal class MoneyIndicator : Script
        {

            GameObject moneyIndicator = new GameObject("HighscoreText");


            public override void Start()
            {

                //set player highscore
                Player.displayedHighscore = PlayerData.Instance.TotalScore;


                var text = moneyIndicator.AddComponent<TextRenderer>();
                moneyIndicator.transform.position = new Vec2D(GetCamera().GetVisibleWidth() - 100, 100);
                text.anchorPoint = AnchorPoint.CenterRight;
                text.SetText(PlayerData.Instance.Money.ToString());
                text.SetColor(SDL2Engine.Color.White);
                text.SetFontSize(48);
                text.SetFontPath("Assets/Fonts/PressStartRegular.ttf");

            }

            public override void Update()
            {

                if (Player.displayedMoney < PlayerData.Instance.Money)
                {
                    Player.displayedMoney += 1;
                }
                else if (Player.displayedMoney > PlayerData.Instance.Money)
                {
                    Player.displayedMoney -= 1;
                }

                //update moneyIndicator
                var text = moneyIndicator.GetComponent<TextRenderer>();
                text?.SetText(Player.displayedMoney.ToString() + "$");
                moneyIndicator.transform.position = new Vec2D(GetCamera().GetVisibleWidth() - 100, 100);

            }
        }

        internal class ShieldTexture : Script
        {

            GameObject shield = new GameObject("Shield");


            public override void Start()
            {

                var sprite = shield.AddComponent<SpriteRenderer>();
                sprite.SetTexture("Assets/Textures/powerups/blue_circle.png");
                sprite.SetWorldSize(200, 200);

            }

            public override void Update()
            {
                var sprite = shield.GetComponent<SpriteRenderer>();
                if (Player.hasShield)
                {
                    sprite?.SetWorldSize(150, 150);
                }
                else
                {
                    sprite?.SetWorldSize(0, 0);
                }
                var player = Find("Player");
                if (player != null)
                {
                    shield.transform.position = player.transform.position;
                }

            }
        }

        internal class WaveIndicator : Script
        {

            GameObject waveIndicator = new GameObject("WaveIndicator");


            public override void Start()
            {

                //set player highscore
                Player.displayedHighscore = PlayerData.Instance.TotalScore;


                var text = waveIndicator.AddComponent<TextRenderer>();
                waveIndicator.transform.position = new Vec2D(100, 100);
                text.anchorPoint = AnchorPoint.CenterLeft;
                text.SetText(PlayerData.Instance.LevelProgress.ToString());
                text.SetColor(SDL2Engine.Color.White);
                text.SetFontSize(48);
                text.SetFontPath("Assets/Fonts/PressStartRegular.ttf");

            }

            public override void Update()
            {

                //update moneyIndicator
                var text = waveIndicator.GetComponent<TextRenderer>();
                text?.SetText("Wave:" + PlayerData.Instance.LevelProgress.ToString());
            }
        }

        internal class HighScore : Script
        {

            GameObject highscoreText = new GameObject("HighscoreText");


            public override void Start()
            {

                //set player highscore
                Player.displayedHighscore = PlayerData.Instance.TotalScore;


                var text = highscoreText.AddComponent<TextRenderer>();
                
                highscoreText.transform.position = new Vec2D(GetCamera().GetVisibleWidth() - 100, 100);
                text.anchorPoint = AnchorPoint.CenterRight;
                text.SetText(PlayerData.Instance.TotalScore.ToString());
                text.SetColor(SDL2Engine.Color.White);
                text.SetFontSize(48);
                text.SetFontPath("Assets/Fonts/PressStartRegular.ttf");

            }

            public override void Update()
            {

                if (Player.displayedHighscore < PlayerData.Instance.TotalScore)
                {
                    Player.displayedHighscore += 1;
                }
                else if (Player.displayedHighscore > PlayerData.Instance.TotalScore)
                {
                    Player.displayedHighscore -= 1;
                }

                //update highscoreText
                var text = highscoreText.GetComponent<TextRenderer>();
                text?.SetText(Player.displayedHighscore.ToString());

                highscoreText.transform.position = new Vec2D(GetCamera().GetVisibleWidth() - 100, 100);

            }
        }


        internal class HealthBar : Script
        {
            GameObject healthBarBackground = new GameObject("HealthBarBackground");
            GameObject healthBarBorder = new GameObject("HealthBarBorder");
            GameObject healthBarText = new GameObject("HealthBarText");
            GameObject? titleText = null;
            GameObject? heart = null;
            int width = 300;
            int height = 60;
            
            public static SDL2Engine.Color backgroundColor = new SDL2Engine.Color(255, 0, 0, 255);

            public override void Start()
            {
                var camera = GetCamera();

                healthBarBackground.transform.position = new Vec2D(100, camera.GetVisibleHeight() - 100);
                var healthIndicator = healthBarBackground.AddComponent<TextRenderer>();
                healthIndicator.anchorPoint = AnchorPoint.CenterLeft;
                healthIndicator.SetPreferredSize(new Rect(0, 0, width, height));
                healthIndicator.SetColor(SDL2Engine.Color.Green);
                healthIndicator.SetBackgroundColor(backgroundColor);



                var border = healthBarBorder.AddComponent<TextRenderer>();
                var textRenderHelper = healthBarBorder.AddComponent<TextRenderHelper>();
                healthBarBorder.transform.position = new Vec2D(100, camera.GetVisibleHeight() - 100);
                border.anchorPoint = AnchorPoint.CenterLeft;
                border.SetPreferredSize(new Rect(0, 0, width, height));
                border.SetBorderSize(2);
                border.SetBorderColor(SDL2Engine.Color.Black);

                var text = healthBarText.AddComponent<TextRenderer>();
                healthBarText.transform.position = new Vec2D(100 + width / 2, camera.GetVisibleHeight() - 100);
                text.anchorPoint = AnchorPoint.Center;
                text.SetText(Player.currentHealth.ToString());
                text.SetColor(SDL2Engine.Color.White);
                text.SetFontSize(38);
                text.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

                titleText = new GameObject("TitleText");
                var text2 = titleText.AddComponent<TextRenderer>();
                titleText.transform.position = new Vec2D(100 + width / 2, camera.GetVisibleHeight() - 150);
                text2.anchorPoint = AnchorPoint.Center;
                text2.SetText("Health");
                text2.SetColor(SDL2Engine.Color.Red);
                text2.SetFontSize(50);
                text2.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");


                heart = new GameObject("Heart");
                var heartTexture = heart.AddComponent<TextureRenderer>();
                heartTexture.relativeToCamera = false;
                heartTexture.SetSource("Assets/Textures/health.png");
                heartTexture.IsVisible(new Rect(4000, 4000));
                heart.transform.position = new Vec2D(110, camera.GetVisibleHeight() - 100);

            }

            public override void Update()
            {
                double currentHealth = Player.currentHealth;
                double maxHealth = Player.maxHealth;
                double healthBarWidth = currentHealth / maxHealth * width;
                var healthIndicator = healthBarBackground.GetComponent<TextRenderer>();
                healthIndicator?.SetRect(new Rect(0, 0, healthBarWidth, height));
                healthIndicator?.SetBackgroundColor(backgroundColor);

                //update the text
                var text = healthBarText.GetComponent<TextRenderer>();
                text?.SetText(Player.currentHealth.ToString());

                var camera = GetCamera();

                healthBarBackground.transform.position = new Vec2D(100, camera.GetVisibleHeight() - 100);
                healthBarBorder.transform.position = new Vec2D(100, camera.GetVisibleHeight() - 100);
                healthBarText.transform.position = new Vec2D(100 + width / 2, camera.GetVisibleHeight() - 100);
                if (titleText == null || heart == null) return;
                titleText.transform.position = new Vec2D(100 + width / 2, camera.GetVisibleHeight() - 150);
                heart.transform.position = new Vec2D(110, camera.GetVisibleHeight() - 100);

                if (Player.hasShield)
                {
                    HealthBar.backgroundColor = new SDL2Engine.Color(0, 0, 255, 255);
                }
                else {       
                    HealthBar.backgroundColor = new SDL2Engine.Color(255, 0, 0, 255);
                }

            }
        }

    }
}
