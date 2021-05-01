using GameEngine.ItemTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Tile
    {
        public string type;
        public Color color = Color.White;
        public Vector2 size;
        public float scale = 1f;
        public bool active = true;
        public int hitPoints;

        public Tile()
        {
            this.type = null;
        }

        public Tile(string type, int hitPoints = 100, int sizeX = 1, int sizeY = 1)
        {
            this.type = type;
            this.hitPoints = hitPoints;
            this.size = new Vector2(sizeX, sizeY);
        }
    }
}
