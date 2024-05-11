using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Coro
{

    // Coroutines system similar to Unity's
    // Use IEnumerator to define a coroutine, with yield return ... for waiting
    public static class CoroutineManager
    {
        
    }

    // Used as yield return in coroutines to wait for a certain amount of time
    // or until a condition is met
    public class CoroDelay
    {
        private double _expectedTime;
        private bool _conditionMet;
        private double _startTime;


        private CoroDelay() {
            _expectedTime = 0;
            _conditionMet = false;
            _startTime = 0;
        }

        public static CoroDelay Frames(int frames)
        {
            CoroDelay delay = new CoroDelay();
            delay._expectedTime = Time.time + Time.GetFPS() * frames;
            delay._startTime = Time.time;
            return delay;
        }

        public static CoroDelay Seconds(double seconds)
        {
            CoroDelay delay = new CoroDelay();
            delay._expectedTime = Time.time + seconds;
            delay._startTime = Time.time;
            return delay;
        }

        public static CoroDelay Until(Func<bool> condition)
        {
            CoroDelay delay = new CoroDelay();
            delay._conditionMet = false;
            delay._startTime = Time.time;
            delay._expectedTime = Time.time;
            return delay;
        }


    }
}
