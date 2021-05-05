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
        public bool collides = true;
        public bool isWood = false;
        public int hitPoints;
        public int frameX = 0;
        public int frameY = 0;
        public int lightLevel = 0;

        public Tile()
        {
            this.type = null;
        }

        public Tile(string type, bool collides = true, int hitPoints = 100, int sizeX = 1, int sizeY = 1, bool isWood = false)
        {
            this.sprite = Sprites.GetSprite(0, type);
            this.type = type;
            this.collides = collides;
            this.hitPoints = hitPoints;
            this.size = new Vector2(sizeX, sizeY);
            this.isWood = isWood;
        }

        public void Draw(SpriteBatch batch, Vector2 pos)
        {
            int cs = GameDemo.chunkSize;

            for (int i = 0; i < size.X; i++)
                for (int j = 0; j < size.Y; j++)
                {
                    Vector2 drawPos = new Vector2(i * cs + pos.X * cs, (pos.Y * cs + j * cs) - (size.Y - 1) * cs);
                    Rectangle spriteSize = new Rectangle();

                    if (size.X == 1 && size.Y == 1)
                        spriteSize = new Rectangle(GetTileState(pos) + frameX, frameY, cs, cs);
                    else
                        spriteSize = new Rectangle(i * cs + frameX, j * cs + frameY, cs, cs);

                    batch.Draw(sprite, drawPos, spriteSize, new Color(lightLevel, lightLevel, lightLevel));
                    this.lightLevel = 0;
                }
        }

        public int GetTileState(Vector2 pos)
        {
            int cs = GameDemo.chunkSize;
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
                return 1 * cs;
            if (up && down && right)
                return 0;
            if (up && down && left)
                return 2 * cs;
            if (right && down && left)
                return 3 * cs;
            if (right && up && left)
                return 4 * cs;
            if (left && down)
                return 8 * cs;
            if (right && down)
                return 7 * cs;
            if (left && up)
                return 6 * cs;
            if (right && up)
                return 5 * cs;
            if (left && right)
                return 13 * cs;
            if (up && down)
                return 14 * cs;
            if (left)
                return 9 * cs;
            if (right)
                return 10 * cs;
            if (up)
                return 11 * cs;
            if (down)
                return 12 * cs;
            if (!down && !up && !right && !left)
                return 15 * cs;

            return 15 * cs;
        }
    }
}
