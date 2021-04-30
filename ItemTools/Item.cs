using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.ItemTools
{
    public class Item
    {
        public string type;
        public string block;
        public Texture2D sprite;

        public int amount;
        public int miningPower;
        public int waitTime;

        public Item(string type, int amount = 1, int waitTime = 1, int miningPower = 0, string block = null)
        {
            this.sprite = Sprites.GetSprite(1, type);
            this.type = type;
            this.amount = amount;
            this.miningPower = miningPower;
            this.waitTime = waitTime;
            this.block = block;
        }

        public Item()
        {

        }
    }
}
