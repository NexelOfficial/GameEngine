using GameEngine.ItemTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Recipe
    {
        public Item result;
        public List<Item> materials;

        public Vector2 position;

        public Recipe(Item result, List<Item> materials, Vector2 position)
        {
            this.result = result;
            this.materials = materials;
            this.position = position;
        }

        public Recipe(Item result, List<Item> materials)
        {
            this.result = result;
            this.materials = materials;
        }

        public void Draw(SpriteBatch batch)
        {
            new ItemSlot(new Vector2(position.X, position.Y), result, 1, true).Draw(batch);

            // Draw materials
            if (MouseEntered())
                foreach (Item material in materials)
                {
                    int matPos = materials.IndexOf(material);
                    float posX = GameDemo.screenWidth / 2 - 294;
                    float posY = GameDemo.screenHeight / 2 - 222;

                    Vector2 itemPos = new Vector2(posX, posY + matPos * 64);

                    new ItemSlot(itemPos, material, 0.8f, true).Draw(batch);
                }
        }

        public bool IsClicked()
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.X > position.X && mouse.X < position.X + 512.0f)
                if (mouse.Y > position.Y && mouse.Y < position.Y + 64.0f)
                {
                    if (Controls.IsPressed("LeftButton"))
                        return true;
                }

            return false;
        }

        public bool MouseEntered()
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.X > position.X && mouse.X < position.X + 64.0f)
                if (mouse.Y > position.Y && mouse.Y < position.Y + 64.0f)
                    return true;

            return false;
        }
    }
}
