﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.ItemTools
{
    public static class Items
    {
        public static Dictionary<string, Item> allItems = new Dictionary<string, Item>();

        public static void InitItems()
        {
            allItems.Add("Grass", new Item("Grass_Item", 1, 1, 0, "Grass"));
            allItems.Add("Stone", new Item("Stone_Item", 1, 1, 0, "Stone"));
            allItems.Add("Sand", new Item("Sand_Item", 1, 1, 0, "Sand"));
            allItems.Add("IronOre", new Item("IronOre_Item", 1, 1, 0));
            allItems.Add("GoldOre", new Item("GoldOre_Item", 1, 1, 0));
            allItems.Add("SilverOre", new Item("SilverOre_Item", 1, 1, 0));
            allItems.Add("Iron_Pickaxe", new Item("Iron_Pickaxe", 1, 2, 60));
        }

        public static Item GetItem(string item)
        {
            if (allItems.ContainsKey(item))
            {
                Item j = allItems[item];
                Item i = new Item(item);

                i.amount = j.amount;
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