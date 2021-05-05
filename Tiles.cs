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
            allTiles.Add("BirchWood", new Tile("BirchWood", true, 100));
            allTiles.Add("BirchLog", new Tile("BirchLog", false, 600, 1, 1, true));
            allTiles.Add("BirchTop", new Tile("BirchTop", false, 600, 12, 12));
            allTiles.Add("Grass", new Tile("Grass", true, 100));
            allTiles.Add("Stone", new Tile("Stone", true, 200));
            allTiles.Add("Sand", new Tile("Sand", true, 80));
            allTiles.Add("IronOre", new Tile("IronOre", true, 300));
            allTiles.Add("GoldOre", new Tile("GoldOre", true, 300));
            allTiles.Add("SilverOre", new Tile("SilverOre", true, 300));
            allTiles.Add("Furnace", new Tile("Furnace", false, 400, 3, 2));
        }

        public static Tile GetTile(string item)
        {
            if (allTiles.ContainsKey(item))
            {
                Tile j = allTiles[item];
                Tile i = new Tile(item);

                i.type = j.type;
                i.collides = j.collides;
                i.hitPoints = j.hitPoints;
                i.color = j.color;
                i.size = j.size;
                i.isWood = j.isWood;
                return i;
            }

            return null;
        }
    }
}
