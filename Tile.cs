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
        public Texture2D sprite;
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
            this.sprite = Sprites.GetSprite(0, type);
            this.type = type;
            this.hitPoints = hitPoints;
            this.size = new Vector2(sizeX, sizeY);
        }

        public void Draw(SpriteBatch batch, Vector2 pos, Color color)
        {
            for (int i = 0; i < size.X; i++)
                for (int j = 0; j < size.Y; j++)
                {
                    Vector2 drawPos = new Vector2(i * 8 + pos.X * 8, (pos.Y * 8 + j * 8) - (size.Y - 1) * 8);
                    Rectangle spriteSize = new Rectangle();

                    if (size.X == 1 && size.Y == 1)
                        spriteSize = new Rectangle(GetTileState(pos), 0, 8, 8);
                    else
                        spriteSize = new Rectangle(i * 8, j * 8, 8, 8);

                    batch.Draw(sprite, drawPos, spriteSize, color);
                }
        }

        public int GetTileState(Vector2 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;

            bool up = false, down = false, left = false, right = false;

            if (GameDemo.GetTile(x, y - 1).size.X == 1)
                up = true;
            if (GameDemo.GetTile(x, y + 1).size.X == 1)
                down = true;
            if (GameDemo.GetTile(x + 1, y).size.X == 1)
                right = true;
            if (GameDemo.GetTile(x - 1, y).size.X == 1)
                left = true;


            if (left && right && up && down)
                return 1 * 8;
            if (up && down && right)
                return 0;
            if (up && down && left)
                return 2 * 8;
            if (right && down && left)
                return 3 * 8;
            if (right && up && left)
                return 4 * 8;
            if (left && down)
                return 8 * 8;
            if (right && down)
                return 7 * 8;
            if (left && up)
                return 6 * 8;
            if (right && up)
                return 5 * 8;
            if (left && right)
                return 13 * 8;
            if (up && down)
                return 14 * 8;
            if (left)
                return 9 * 8;
            if (right)
                return 10 * 8;
            if (up)
                return 11 * 8;
            if (down)
                return 12 * 8;
            if (!down && !up && !right && !left)
                return 15 * 8;

            return 15 * 8;
        }
    }
}
