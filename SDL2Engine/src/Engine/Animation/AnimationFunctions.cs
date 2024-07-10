using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Animation
{

    public enum SmoothType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad
    }
    public static class AnimationFunctions
    {
        public static double Linear(double start, double end, double t)
        {
            return start + (end - start) * t;
        }

        public static double Linear(double end, double t)
        {

           return end * t;
        }

        public static double EaseInQuad(double start, double end, double t)
        {
            return start + (end - start) * t * t;
        }

        public static double EaseInQuad(double end, double t)
        {
            return end * t * t;
        }

        public static double EaseOutQuad(double start, double end, double t)
        {
            return start + (end - start) * t * (2 - t);
        }

        public static double EaseOutQuad(double end, double t)
        {
            return end * t * (2 - t);
        }

        public static double EaseInOutQuad(double start, double end, double t)
        {
            if (t < 0.5)
            {
                return start + (end - start) * 2 * t * t;
            }
            else
            {
                return start + (end - start) * (1 - 2 * (1 - t) * (1 - t));
            }
        }

        public static double EaseInOutQuad(double end, double t)
        {
            if (t < 0.5)
            {
                return end * 2 * t * t;
            }
            else
            {
                return end * (1 - 2 * (1 - t) * (1 - t));
            }
        }

    }
}
