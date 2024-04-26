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
    public class TestEngine
    {
       
    }
}
