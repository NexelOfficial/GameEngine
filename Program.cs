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
    }
}
