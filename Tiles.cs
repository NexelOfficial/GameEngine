using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.ItemTools
{
    public class Tiles
    {
        public static Dictionary<string, Tile> allTiles = new Dictionary<string, Tile>();

        public static void InitTiles()
        {
            allTiles.Add("Grass", new Tile("Grass", 100));
            allTiles.Add("Stone", new Tile("Stone", 200));
            allTiles.Add("Sand", new Tile("Sand", 80));
            allTiles.Add("IronOre", new Tile("IronOre", 300));
            allTiles.Add("GoldOre", new Tile("GoldOre", 300));
            allTiles.Add("SilverOre", new Tile("SilverOre", 300));
            allTiles.Add("Furnace", new Tile("Furnace", 400, 3, 2));
        }

        public static Tile GetTile(string item)
        {
            if (allTiles.ContainsKey(item))
            {
                Tile j = allTiles[item];
                Tile i = new Tile(item);

                i.type = j.type;
                i.hitPoints = j.hitPoints;
                i.color = j.color;
                i.size = j.size;
                return i;
            }

            Console.WriteLine(item);
            return null;
        }
    }
}
