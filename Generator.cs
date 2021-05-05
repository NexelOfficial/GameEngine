using GameEngine.ItemTools;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Generator
    {
        private Random rand;
        private Noise noise = new Noise();
        public int seed;

        public void DigTileBlob(Vector2 pos, int radius, int steps, float spreadX = 0f, float spreadY = 0f)
        {
            Noise noise = new Noise();
            int noiseFactor = rand.Next(radius - 1, radius + 2);

            while (steps > 0)
            {
                for (int x = (int)pos.X - radius; x < pos.X + radius; x++)
                {
                    for (int y = (int)pos.Y - radius; y < pos.Y + radius; y++)
                    {
                        float spread = Math.Abs(noise.GetNext(x, seed, noiseFactor)) * 1.5f;

                        if (Vector2.Distance(new Vector2(x, y), pos) <= radius + spread)
                            GameDemo.RemoveTile(x, y);
                    }
                }

                pos.X += (noise.GetNext(pos.Y / (radius * 2) + rand.Next(5, 10), seed, radius) - spreadX) * (radius * 2);
                pos.Y += (noise.GetNext(rand.Next(20, 200), seed, radius) - spreadY) * (radius * 2);
                steps--;
            }
        }

        public void PlaceTileBlob(Vector2 pos, string type, int radius, int steps, float spreadX = 0f, float spreadY = 0f)
        {
            Noise noise = new Noise();
            int noiseFactor = rand.Next(radius - 1, radius + 2);

            while (steps > 0)
            {
                for (int x = (int)pos.X - radius; x < pos.X + radius; x++)
                {
                    for (int y = (int)pos.Y - radius; y < pos.Y + radius; y++)
                    {
                        float spread = noise.GetNext(x, seed, noiseFactor) * 1.5f;

                        if (Vector2.Distance(new Vector2(x, y), pos) <= radius + spread)
                            GameDemo.AddTile(x, y, Tiles.GetTile(type));
                    }
                }

                pos.X += (noise.GetNext(pos.Y / (radius * 2) + rand.Next(5, 10), seed, radius) - spreadX) * (radius * 2);
                pos.Y += (noise.GetNext(rand.Next(20, 200), seed, radius) - spreadY) * (radius * 2);
                steps--;
            }
        }

        public void GenerateTerrain(int width, int height)
        {
            rand = new Random(seed);

            genDirtStone(width);
            genDeserts(width);
            genCaves(width, height);
            genOres(width, height);
            genTrees(width);
        }

        private void genDirtStone(int width)
        {
            for (int x = 0; x < width; x++)
            {
                int y = (int)(noise.GetNext(x + rand.Next(-1, 1), seed, 70) * 20) + 40;
                int y2 = y;

                while (y < GameDemo.mapHeight)
                {
                    if (y > y2 + 10 + noise.GetNext(x, seed, 50) * 6)
                        GameDemo.AddTile(new Vector2(x, y), Tiles.GetTile("Stone"));
                    else
                        GameDemo.AddTile(new Vector2(x, y), Tiles.GetTile("Grass"));
                    y++;
                }
            }
        }

        private void genDeserts(int width)
        {
            for (int i = 0; i < rand.Next(4, 7); i++)
            {
                int desertBiomeStart = rand.Next(0, width);
                int desertSize = rand.Next(90, 135);

                while (desertBiomeStart - desertSize < 0 || desertBiomeStart + desertSize > width)
                {
                    desertBiomeStart = rand.Next(0, width);
                    desertSize = rand.Next(90, 135);
                }

                while (desertBiomeStart > (width / 2) - 150 && desertBiomeStart < (width / 2) + 150)
                {
                    desertBiomeStart = rand.Next(0, width);
                    desertSize = rand.Next(90, 135);
                }

                Console.WriteLine(desertBiomeStart);

                int endY = 0;
                for (int x = desertBiomeStart - desertSize; x < desertBiomeStart + desertSize; x++)
                {
                    int y = GameDemo.GetHighestY(x);

                    if (x < desertBiomeStart - 10)
                        endY += rand.Next(1, 3);
                    if (x > desertBiomeStart + 10)
                        endY -= rand.Next(1, 3);

                    while (y < endY)
                    {
                        GameDemo.AddTile(new Vector2(x, y), Tiles.GetTile("Sand"));
                        y++;
                    }
                }
            }
        }

        private void genCaves(int width, int height)
        {
            // Deep caves
            for (int x = 0; x < width; x++)
            {
                if (rand.Next(0, 15) == 0)
                    for (int i = 0; i < rand.Next(2, 3); i++)
                    {
                        int y = height - rand.Next(0, (int)(height * 0.88));

                        DigTileBlob(new Vector2(x, y), rand.Next(4, 5), rand.Next(80, 230), 0f, -0.05f);
                    }
            }

            // Surface caves
            for (int x = 0; x < width; x++)
            {
                if (rand.Next(0, 200) == 0)
                {
                    if (x > (width / 2) - 150 && x < (width / 2) + 150)
                        continue;

                    int y = GameDemo.GetHighestY(x);

                    DigTileBlob(new Vector2(x, y), rand.Next(4, 5), rand.Next(150, 400), 0f, -0.22f);
                }
            }
        }

        private void genOres(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                // Iron Ore
                for (int i = 0; i < rand.Next(1, 5); i++)
                {
                    int y = rand.Next((int)(height * 0.2), height);

                    if (GameDemo.GetTile(x, y).type != null)
                        PlaceTileBlob(new Vector2(x, y), "IronOre", rand.Next(2, 4), rand.Next(0, 3));
                }

                // Silver Ore
                for (int i = 0; i < rand.Next(0, 5); i++)
                {
                    int y = rand.Next((int)(height * 0.45), height);

                    if (GameDemo.GetTile(x, y).type != null)
                        PlaceTileBlob(new Vector2(x, y), "SilverOre", rand.Next(2, 3), rand.Next(0, 2));
                }

                // Gold Ore
                for (int i = 0; i < rand.Next(0, 4); i++)
                {
                    int y = rand.Next((int)(height * 0.3), height);

                    if (GameDemo.GetTile(x, y).type != null)
                        PlaceTileBlob(new Vector2(x, y), "GoldOre", rand.Next(2, 3), rand.Next(0, 2));
                }
            }
        }

        private void genTrees(int width)
        {
            Tile birchLog = Tiles.GetTile("BirchLog");
            for (int x = 0; x < width; x++)
            {
                if (rand.Next(0, 10) == 0)
                {
                    int height = rand.Next(8, 13);
                    int highestY = GameDemo.GetHighestY(x);

                    // Check if tree can be placed
                    if (!GameDemo.IsEmpty(new Vector2(x-11, highestY - height - 11), 11, 11 + height-2))
                        continue;

                    if (!GameDemo.IsEmpty(new Vector2(x-1, highestY - 2), 4, 2))
                        continue;

                    if (GameDemo.GetTile(x, highestY).type != "Grass" || GameDemo.GetTile(x + 1, highestY).type != "Grass")
                        continue;

                    // Add wood
                    for (int y = highestY - height; y < highestY; y++)
                    {
                        GameDemo.AddTile(x, y, birchLog);
                        GameDemo.AddTile(x + 1, y, birchLog);
                    }
                    Tile treeTop = Tiles.GetTile("BirchTop");
                    treeTop.frameX = rand.Next(0, 3) * 96;
                    GameDemo.AddTile(x - 5, highestY - height - 1, treeTop);

                    // Add grass to bottom
                    for (int i = 0; i < 4; i++)
                        GameDemo.AddTile(x - 1 + i, highestY, Tiles.GetTile("Grass"));
                }
            }
        }
    }
}
