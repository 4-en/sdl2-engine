using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp
{
    public class GameText : TextRenderer
    {
        public double duration = 2;
        public double timeAlive = 0;
        public Vec2D velocity = new Vec2D(0, -50);

        public GameText()
        {
            this.relativeToCamera = true;
            this.z_index = -2;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);
            timeAlive += Time.deltaTime;
            if (timeAlive >= duration)
            {
                gameObject.Destroy();
            }

            gameObject.transform.Move(velocity * Time.deltaTime);
        }

        public static GameObject CreateAt(Vec2D position, string text, double duration=2, int fontSize=24, Color? color = null)
        {
            var textObject = new GameObject(text);
            textObject.transform.position = position;
            var textComponent = textObject.AddComponent<GameText>();
            textComponent.color = color ?? new Color(255, 255, 255, 255);
            textComponent.SetFontSize(fontSize);
            textComponent.SetText(text);
            textComponent.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            textComponent.duration = duration;
            return textObject;
        }
    }
}
