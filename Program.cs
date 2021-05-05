using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (GameDemo game = new GameDemo())
            {
                game.Run();
            }
        }

        [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
        private static extern int LoadNvApi64();

        [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
        private static extern int LoadNvApi32();

        public static void TryForceHighPerformanceGpu()
        {
            try
            {
                if (System.Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();
            }
            catch { } // this will always be triggered, so just catch it and do nothing :P
        }
    }
}
