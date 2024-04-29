using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    public static class Time
    {

        // all time values are in seconds
        public static ulong tick = 0;
        public static double time = 0;
        public static double deltaTime = 0;

        public static double lastDrawTime = 0;
        public static double lastUpdateTime = 0;

        public static double updateDuration = 0;
        public static double drawDuration = 0;
        public static double totalDuration = 0;
        public static double freeDuration = 0;

        public static double GetFPS()
        {
            return 1.0 / deltaTime;
        }
    }

}
