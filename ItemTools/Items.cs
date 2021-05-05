using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.ItemTools
{
    public class Items
    {
        public static Dictionary<string, Item> allItems = new Dictionary<string, Item>();

        public static void InitItems()
        {
            allItems.Add("Grass", new Item("Grass", 1, 1, 0, "Grass"));
            allItems.Add("Stone", new Item("Stone", 1, 1, 0, "Stone"));
            allItems.Add("Sand", new Item("Sand", 1, 1, 0, "Sand"));
            allItems.Add("IronOre", new Item("IronOre"));
            allItems.Add("GoldOre", new Item("GoldOre"));
            allItems.Add("SilverOre", new Item("SilverOre"));
            allItems.Add("IronBar", new Item("IronBar"));
            allItems.Add("GoldBar", new Item("GoldBar"));
            allItems.Add("Furnace", new Item("Furnace", 1, 1, 0, "Furnace"));
            allItems.Add("Iron_Pickaxe", new Item("Iron_Pickaxe", 1, 2, 60));
        }

        public static Item GetItem(string item, int amount = 1)
        {
            if (allItems.ContainsKey(item))
            {
                Item j = allItems[item];
                Item i = new Item(item);

                i.amount = amount;
                i.block = j.block;
                i.miningPower = j.miningPower;
                i.sprite = j.sprite;
                i.type = j.type;
                i.waitTime = j.waitTime;
                return i;
            }

            return null;
        }
    }
}
