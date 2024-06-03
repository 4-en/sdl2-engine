using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ShootEmUp
{
    public static class UI
    {
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

            var button2Tuple = Button("Main Menu", () => { LevelManager.LoadHomeScreen(); return true; }, new Rect(0, 0, 350, 150), Color.White, 44);
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
            menu.AddComponent<ShopKeyListener>().OnKeyP = () =>
            {
                menu.Destroy();
                onContinue?.Invoke();
                return true;
            };

            return menu;
        }

        //shop menu
        public static GameObject ShopMenu(string title = "Shop", Func<bool>? onContinue = null)
        {
            //test values
            var playerData = PlayerData.Instance;
            /*
            playerData.SpeedUpgradeLevel = 0;
            playerData.DamageUpgradeLevel = 0;
            playerData.HealthUpgradeLevel = 0;
            playerData.Money = 10000;
            playerData.TotalScore = 500;*/
            var playerMoney = playerData.Money;
            var speedUpgrade = playerData.SpeedUpgradeLevel;
            var damageUpgrade = playerData.DamageUpgradeLevel;
            var healthUpgrade = playerData.HealthUpgradeLevel;



            var menu = new GameObject("ShopMenu");
            menu.SetPosition(new Vec2D(1920 / 2, 400));

            var titleTextTuple = Component.CreateWithGameObject<TextRenderer>("ShopMenuTitle");
            var titleText = titleTextTuple.Item2;
            titleTextTuple.Item1.SetPosition(new Vec2D(0, -250));
            titleText.SetFontSize(78);
            titleText.SetText(title);
            titleText.SetPreferredSize(new Rect(0, 0, 500, 200));
            titleText.anchorPoint = AnchorPoint.Center;
            titleText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            menu.AddChild(titleTextTuple.Item1);


            //money indicator at the top left
            var moneyTextTuple = Component.CreateWithGameObject<TextRenderer>("MoneyText");
            var moneyText = moneyTextTuple.Item2;
            moneyTextTuple.Item1.SetPosition(new Vec2D(-900, -250));
            moneyText.SetFontSize(78);
            moneyText.SetText("Balance    " + playerMoney);
            moneyText.SetPreferredSize(new Rect(0, 0, 500, 200));
            moneyText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            moneyText.anchorPoint = AnchorPoint.CenterLeft;
            menu.AddChild(moneyTextTuple.Item1);

            //highscore indicator at the top right
            var highscoreTextTuple = Component.CreateWithGameObject<TextRenderer>("HighscoreText");
            var highscoreText = highscoreTextTuple.Item2;
            highscoreTextTuple.Item1.SetPosition(new Vec2D(900, -250));
            highscoreText.SetFontSize(78);
            highscoreText.SetText("Highscore    " + playerData.TotalScore);
            highscoreText.SetPreferredSize(new Rect(0, 0, 500, 200));
            highscoreText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            highscoreText.anchorPoint = AnchorPoint.CenterRight;
            menu.AddChild(highscoreTextTuple.Item1);



            ShopItem(menu, "Speed", new Vec2D(-500, 100),PlayerData.Instance.SpeedUpgradeLevel);

            ShopItem(menu, "Damage", new Vec2D(0, 100),PlayerData.Instance.DamageUpgradeLevel);

            ShopItem(menu, "Health", new Vec2D(500, 100),PlayerData.Instance.HealthUpgradeLevel);

            var continueButtonTuple = Button("Continue", () => { LevelManager.LoadHomeScreen(); return true; }, new Rect(0, 0, 300, 100), Color.White, 44);
            var continueButton = continueButtonTuple.Item1;
            continueButton.SetLocalPosition(new Vec2D(750, 575));
            menu.AddChild(continueButton);

            return menu;
        }

        private static void ShopItem(GameObject menu, String title, Vec2D position, int upgradeLvl)
        {
            

            var titleTextTuple = Component.CreateWithGameObject<TextRenderer>("EscapeMenuTitle");
            var titleText = titleTextTuple.Item2;
            titleText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            titleTextTuple.Item1.SetPosition(position);
            titleText.SetFontSize(64);
            titleText.SetText(title);
            titleText.SetPreferredSize(new Rect(0, 0, 500, 200));
            titleText.anchorPoint = AnchorPoint.Center;
            menu.AddChild(titleTextTuple.Item1);

            

            //5 buttons without text which identify the level of the upgrade

            var firstUpgradeTuple = Button("", () =>
            {
                return true;
            }, new Rect(0, 0, 75, 75), Color.White, 44);
            var firstUpgradeButton = firstUpgradeTuple.Item1; 
            firstUpgradeButton.SetLocalPosition(position + new Vec2D(-160,75));
            firstUpgradeButton.RemoveComponent<TextRenderHelper>();
            if (upgradeLvl>=1)
            {
                firstUpgradeButton.GetComponent<TextRenderer>()?.SetBackgroundColor(new Color(0, 255, 0, 100));
            }
            menu.AddChild(firstUpgradeButton);

            var secondUpgradeTuple = Button("", () =>
            {
                return true;
            }, new Rect(0, 0, 75, 75), Color.White, 44);
            var secondUpgradeButton = secondUpgradeTuple.Item1;
            secondUpgradeButton.SetLocalPosition(position + new Vec2D(-79, 75));
            secondUpgradeButton.RemoveComponent<TextRenderHelper>();
            if (upgradeLvl>=2)
            {
                  secondUpgradeButton.GetComponent<TextRenderer>()?.SetBackgroundColor(new Color(0, 255, 0, 100));
            }
            menu.AddChild(secondUpgradeButton);

            var thirdUpgradeTuple = Button("", () =>
            {
                return true;
            }, new Rect(0, 0, 75, 75), Color.White, 44);
            var thirdUpgradeButton = thirdUpgradeTuple.Item1;
            thirdUpgradeButton.SetLocalPosition(position + new Vec2D(2, 75));
            thirdUpgradeButton.RemoveComponent<TextRenderHelper>();
            if(upgradeLvl >= 3)
            {
                thirdUpgradeButton.GetComponent<TextRenderer>()?.SetBackgroundColor(new Color(0, 255, 0, 100));
            }
            menu.AddChild(thirdUpgradeButton);

            var fourthUpgradeTuple = Button("", () =>
            {
                return true;
            }, new Rect(0, 0, 75, 75), Color.White, 44);
            var fourthUpgradeButton = fourthUpgradeTuple.Item1;
            fourthUpgradeButton.SetLocalPosition(position + new Vec2D(83, 75));
            fourthUpgradeButton.RemoveComponent<TextRenderHelper>();
            if (upgradeLvl >= 4)
            {
                fourthUpgradeButton.GetComponent<TextRenderer>()?.SetBackgroundColor(new Color(0, 255, 0, 100));
            }
            menu.AddChild(fourthUpgradeButton);

            var fifthUpgradeTuple = Button("", () =>
            {
                return true;
            }, new Rect(0, 0, 75, 75), Color.White, 44);
            var fifthUpgradeButton = fifthUpgradeTuple.Item1;
            fifthUpgradeButton.SetLocalPosition(position+ new Vec2D(164, 75));
            fifthUpgradeButton.RemoveComponent<TextRenderHelper>();
            if(upgradeLvl >= 5)
            {
                fifthUpgradeButton.GetComponent<TextRenderer>()?.SetBackgroundColor(new Color(0, 255, 0, 100));
            }
            menu.AddChild(fifthUpgradeButton);

            //upgrade button
            var upgradeButtonTuple = Button("1000$", () =>
            {
                if (title.Equals("Speed"))
                {
                    if (PlayerData.Instance.SpeedUpgradeLevel < 5 && PlayerData.Instance.Money >= 1000)
                    {
                        PlayerData.Instance.Money -= 1000;
                        PlayerData.Instance.SpeedUpgradeLevel++;
                    }
                }else if (title.Equals("Damage"))
                {
                    if(PlayerData.Instance.DamageUpgradeLevel < 5 && PlayerData.Instance.Money >= 1000)
                    {
                        PlayerData.Instance.Money -= 1000;
                        PlayerData.Instance.DamageUpgradeLevel++;
                    }
                }else if(title.Equals("Health"))
                {
                    if (PlayerData.Instance.HealthUpgradeLevel < 5 && PlayerData.Instance.Money >= 1000)
                    {
                        PlayerData.Instance.Money -= 1000;
                        PlayerData.Instance.HealthUpgradeLevel++;
                    }
                }
                Console.WriteLine("Speed: " + PlayerData.Instance.SpeedUpgradeLevel+", Damage: "+PlayerData.Instance.DamageUpgradeLevel+", Health: "+PlayerData.Instance.HealthUpgradeLevel);
                LevelManager.LoadShop();
                return true;
            }, new Rect(0, 0, 400, 75), Color.White, 44);
            var upgradeButton = upgradeButtonTuple.Item1;
            upgradeButton.SetLocalPosition(position+new Vec2D(2, 160));
            menu.AddChild(upgradeButton);


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

        class ShopKeyListener : Script
        {

            public Func<bool>? OnKeyP { get; set; }
            public override void Update()
            {
                if (Input.GetKeyDown(SDL_Keycode.SDLK_p))
                {
                    if (OnKeyP != null)
                    {
                        OnKeyP.Invoke();
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
