using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine.UI;

namespace GameEngine.ItemTools
{
    public class Inventory
    {
        public Player owner;
        public List<ItemSlot> Slots = new List<ItemSlot>();
        public List<Blueprint> UnlockedBlueprints = new List<Blueprint>();

        public BlueprintsMenu blueprints;
        public FurnaceMenu furnace;

        public int selectedIndex;
        public bool inventoryShown = false;
        public bool blueprintsShown = false;
        public bool furnaceShown = false;
        public Tile placingBlueprint;

        public Item heldItem = new Item();

        public Inventory(Player player)
        {
            owner = player;

            for (int i = 0; i <= 54; i++)
                Slots.Add(new ItemSlot(Vector2.Zero, new Item()));

            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 4; j++)
                    Slots[i + (j * 10)] = new ItemSlot(new Vector2(i * 72 + 8, j * 72 + 8), new Item());
            }

            blueprints = new BlueprintsMenu();
        }

        public void SelectSlot()
        {
            Controls.GetState();

            if (Controls.IsPressed(Keys.D1))
                selectedIndex = 0;
            if (Controls.IsPressed(Keys.D2))
                selectedIndex = 1;
            if (Controls.IsPressed(Keys.D3))
                selectedIndex = 2;
            if (Controls.IsPressed(Keys.D4))
                selectedIndex = 3;
            if (Controls.IsPressed(Keys.D5))
                selectedIndex = 4;
            if (Controls.IsPressed(Keys.D6))
                selectedIndex = 5;
            if (Controls.IsPressed(Keys.D7))
                selectedIndex = 6;
            if (Controls.IsPressed(Keys.D8))
                selectedIndex = 7;
            if (Controls.IsPressed(Keys.D9))
                selectedIndex = 8;
            if (Controls.IsPressed(Keys.D0))
                selectedIndex = 9;

            // Toggle inventory
            if (Controls.IsPressed(Keys.E))
                if (inventoryShown)
                    inventoryShown = false;
                else
                {
                    furnace = null;
                    inventoryShown = true;
                    blueprintsShown = false;
                }

            // Toggle blueprints menu
            if (Controls.IsPressed(Keys.B))
                if (blueprintsShown)
                    blueprintsShown = false;
                else
                {
                    furnace = null;
                    inventoryShown = false;
                    blueprintsShown = true;
                }

            // Check if slot is clicked
            foreach (ItemSlot slot in Slots)
            {
                if (heldItem.amount <= 0)
                    heldItem = new Item();
                if (slot.item.amount <= 0)
                    slot.item = new Item();

                if (slot.IsClicked("LeftButton"))
                {
                    Item slotItem = slot.item;

                    if (slot.item.type == heldItem.type)
                    {
                        slot.item.amount += heldItem.amount;
                        heldItem = new Item();
                    }
                    else
                    {
                        slot.item = heldItem;
                        heldItem = slotItem;
                    }
                }

                if (slot.IsClicked("RightButton"))
                {
                    if (slot.item.type == heldItem.type)
                    {
                        slot.item.amount += 1;
                        heldItem.amount -= 1;
                    }
                    else if (heldItem.type == null)
                    {
                        heldItem.type = slot.item.type;
                        heldItem.sprite = slot.item.sprite;
                        heldItem.amount = slot.item.amount / 2;
                        slot.item.amount -= heldItem.amount;
                    }
                    else if (heldItem.type != null && slot.item.type == null)
                    {
                        slot.item.type = heldItem.type;
                        slot.item.sprite = heldItem.sprite;
                        slot.item.amount += 1;
                        heldItem.amount -= 1;
                    }
                }

                // Reset slot
                slot.shown = false;
            }
        }

        public void DrawInventory(SpriteBatch batch)
        {
            // Draw item slots
            for (int i = 0; i <= 9; i++)
            {
                Slots[i].Draw(batch);

                for (int j = 0; j <= 3; j++)
                    if (inventoryShown)
                        Slots[i + (j * 10) + 10].Draw(batch);
            }

            // Draw held item
            if (heldItem.sprite != null)
            {
                MouseState mouse = Mouse.GetState();
                Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

                heldItem.Draw(batch, mousePos);
            }

            // Draw blueprint menu
            if (blueprintsShown && placingBlueprint == null)
                blueprints.Draw(batch);

            // Draw furnace menu
            Vector2 worldSpace = owner.GetMousePosition();
            int blockX = (int)Math.Floor(worldSpace.X / 8) + 1;
            int blockY = (int)Math.Floor(worldSpace.Y / 8) + 2;
            Tile clickedTile = GameDemo.GetTile(blockX, blockY);

            if (clickedTile.type == "Furnace")
                if (GameDemo.furnaces.ContainsKey(new Vector2(blockX, blockY)))
                    if (Controls.IsPressed("RightButton"))
                        furnace = GameDemo.furnaces[new Vector2(blockX, blockY)];

            // Draw furnace menu
            if (furnace != null)
            {
                furnace.Draw(batch);
                furnaceShown = true;
                blueprintsShown = false;
                inventoryShown = false;
            }

            // Close menu(s)
            if (Controls.IsPressed(Keys.Escape))
            {
                furnace = null;
                blueprintsShown = false;
                inventoryShown = false;
            }
        }

        // Callable methods
        public void AddItem(Item item)
        {
            foreach (ItemSlot slot in Slots)
                if (slot.item.type == item.type)
                    while (slot.item.amount + 1 <= 199 && item.amount > 0)
                    {
                        slot.item.amount += 1;
                        item.amount -= 1;
                    }

            if (item.amount > 0)
                foreach (ItemSlot slot in Slots)
                    if (slot.item.sprite == null)
                    {
                        slot.item = item;
                        break;
                    }
        }

        public bool HasItem(string type, int amount)
        {
            foreach (ItemSlot slot in Slots)
                if (slot.item.type == type)
                {
                    int hit = 0;
                    while (slot.item.amount - hit - 1 >= 0 && amount > 0)
                    {
                        hit += 1;
                        amount -= 1;
                    }
                }

            if (amount <= 0)
                return true;
            else
                return false;
        }

        public void RemoveItem(string type, int amount)
        {
            foreach (ItemSlot slot in Slots)
                if (slot.item.type == type)
                    while (slot.item.amount - 1 >= 0 && amount > 0)
                    {
                        slot.item.amount -= 1;
                        amount -= 1;

                        if (slot.item.amount <= 0)
                            slot.item = new Item();
                    }
        }
    }
}
