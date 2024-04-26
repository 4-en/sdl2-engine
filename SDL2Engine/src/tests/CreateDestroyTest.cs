using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Testing
{

    /*
     * This test is used to test the creation and destruction of GameObjects
     * Make sure that GameObjects are properly destroyed and that they are not leaked
     * Also watch out for Component leaks
     * 
     * Test Script specific listeners like Start, Update, and Destroy
     */
    public class CreateDestroyTest
    {

        // when stopping, check that the counts have the expected values
        public static int startCount = 0;
        public static int updateCount = 0;
        public static int destroyCount = 0;
        public static int awakeCount = 0;
        public static int onEnableCount = 0;
        public static int onDisableCount = 0;

        public void Run()
        {
            Console.WriteLine("CreateDestroyTest Run");
        }
    }
}
