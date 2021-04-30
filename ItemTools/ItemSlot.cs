using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.ItemTools
{
    public class ItemSlot
    {
        public Vector2 position;
        public float size;
        public Item item;
        public bool shown = false;

        public ItemSlot(Vector2 position, Item item = null, float size = 1.0f)
        {
            this.position = position;
            this.size = size;
            this.item = item;
        }

        public ItemSlot()
        {
            
        }

        public void Draw(SpriteBatch batch)
        {
            float scaleOffset = ((64.0f * size) - 64.0f) * 0.5f;
            batch.Draw(Sprites.GetSprite(2, "Item_Slot"), position - new Vector2(scaleOffset, scaleOffset), new Rectangle(0, 0, 64, 64), Color.White, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
        }

        public bool IsClicked()
        {
            MouseState mouse = Mouse.GetState();
            float finalSize = 64.0f * size;

            if (mouse.X > position.X && mouse.X < position.X + finalSize)
                if (mouse.Y > position.Y && mouse.Y < position.Y + finalSize)
                {
                    if (Controls.IsPressed("LeftButton") && shown)
                        return true;
                }

            return false;
        }
    }
}
