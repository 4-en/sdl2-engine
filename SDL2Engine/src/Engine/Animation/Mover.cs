using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Animation
{
    internal class Mover : Script
    {

        private Vec2D start;
        private Vec2D end;
        private Vec2D last;

        private double duration;
        private double time = 0;
        private SmoothType smoothType;

        private bool _createdCorrectly = false;

        public static void Move(GameObject target, Vec2D end, double duration = 1.0, SmoothType smoothType = SmoothType.Linear)
        {
            var mover = target.AddComponent<Mover>();
            mover.start = target.GetPosition();
            mover.end = end;
            mover.duration = duration;
            mover.smoothType = smoothType;
            mover._createdCorrectly = true;
        }

        public override void Start()
        {
            if (!_createdCorrectly)
            {
                Destroy();
                return;
            }

            start = gameObject.GetPosition();
            last = start;
        }

        private Func<double, double, double, double> GetSmoothFunction()
        {
            switch (smoothType)
            {
                case SmoothType.Linear:
                    return AnimationFunctions.Linear;
                case SmoothType.EaseInQuad:
                    return AnimationFunctions.EaseInQuad;
                case SmoothType.EaseOutQuad:
                    return AnimationFunctions.EaseOutQuad;
                case SmoothType.EaseInOutQuad:
                    return AnimationFunctions.EaseInOutQuad;
                default:
                    return AnimationFunctions.Linear;
            }
        }

        public override void Update()
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                gameObject.SetPosition(end);
                Destroy();
                return;
            }

            if(last != gameObject.GetPosition())
            {
                // something else moved the object, cancel animation
                Destroy();
                return;
            }

            var smoothFunction = GetSmoothFunction();
            var x = smoothFunction(start.x, end.x, time / duration);
            var y = smoothFunction(start.y, end.y, time / duration);
            var newPos = new Vec2D(x, y);
            gameObject.SetPosition(newPos);
            last = newPos;
        }
    }
}
