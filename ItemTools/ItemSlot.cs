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
        public bool readOnly = false;

        public ItemSlot(Vector2 position, Item item = null, float size = 1.0f, bool readOnly = false)
        {
            this.position = position;
            this.size = size;
            this.item = item;
            this.readOnly = readOnly;
        }

        public ItemSlot()
        {
            this.position = Vector2.Zero;
            this.size = 1.0f;
            this.item = new Item();
        }

        public void Draw(SpriteBatch batch)
        {
            // Draw frame
            float scaleOffset = ((64.0f * size) - 64.0f) * 0.5f;
            batch.Draw(Sprites.GetSprite(2, "Item_Slot"), position - new Vector2(scaleOffset, scaleOffset), new Rectangle(0, 0, 64, 64), Color.White, 0f, Vector2.Zero, size, SpriteEffects.None, 0.3f);
            shown = true;

            // Draw item
            if (item.sprite == null)
                return;

            Vector2 itemPos = new Vector2(position.X + 8, position.Y + 8);
            item.Draw(batch, itemPos);
        }

        public bool IsClicked(string button)
        {
            MouseState mouse = Mouse.GetState();
            float finalSize = 64.0f * size;

            if (mouse.X > position.X && mouse.X < position.X + finalSize)
                if (mouse.Y > position.Y && mouse.Y < position.Y + finalSize)
                {
                    if (Controls.IsPressed(button) && shown)
                        return true;
                }

            return false;
        }
    }
}
