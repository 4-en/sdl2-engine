using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Testing
{
    public abstract class EngineTest
    {
        public abstract void Run();
    }

    // Basically the game engine without any SDL2 calls and no sleep between updates
    public class TestEngine : IEngine
    {

        private Scene? currentScene;
        private int tps = 60;
        private int ticks_left = 0;
        private int sim_tick_count = 1000;

        public TestEngine(Scene? scene = null, int sim_tick_count = 1000)
        {
            this.currentScene = scene;
            this.sim_tick_count = sim_tick_count;
        }

        public void Run()
        {

            if (this.currentScene == null)
            {
                return;
            }

            if (ticks_left > 0)
            {
                return;
            }
            ticks_left = sim_tick_count;


            double secondsPerTick = 1.0 / tps;
            Time.tick = 0;
            Time.time = 0;
            Time.deltaTime = secondsPerTick;
            // Run the engine
            while (ticks_left > 0)
            {
                // Update the engine as fast as possible
                Update();
                Time.tick++;
                Time.time += secondsPerTick;
                Time.lastUpdateTime = Time.time;
                Time.lastDrawTime = Time.time;
                
                ticks_left--;
            }
        }


        public void Update()
        {
            this.currentScene?.Update();
        }

        public void SetScene(Scene scene)
        {
            this.currentScene = scene;
        }
    }
}
