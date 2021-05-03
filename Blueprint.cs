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
    public class Blueprint
    {
        public Tile result;
        public List<Item> materials;

        public Vector2 position;

        public Blueprint(Tile result, List<Item> materials, Vector2 position)
        {
            this.result = result;
            this.materials = materials;
            this.position = position;
        }

        public Blueprint(Tile result, List<Item> materials)
        {
            this.result = result;
            this.materials = materials;
        }

        public void Draw(SpriteBatch batch)
        {
            Texture2D resSprite = Items.GetItem(result.type).sprite;

            batch.Draw(Sprites.GetSprite(2, "Blueprint"), new Vector2(position.X, position.Y), Color.White);
            batch.Draw(resSprite, new Vector2(position.X + 8, position.Y + 8), new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            // Draw materials
            foreach (Item material in materials)
            {
                int matPos = materials.IndexOf(material);

                batch.Draw(material.sprite, new Vector2(position.X + 16 + 64 + matPos * 48, position.Y + 16), new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
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
    }
}
