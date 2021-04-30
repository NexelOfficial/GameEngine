using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.ItemTools
{
    public class Inventory
    {
        public Player owner;
        public List<ItemSlot> Slots = new List<ItemSlot>();
        public int selectedIndex;
        public bool inventoryShown = false;
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
        }

        // Update methods
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

            if (Controls.IsPressed(Keys.E))
                if (inventoryShown)
                    inventoryShown = false;
                else
                    inventoryShown = true;

            foreach (ItemSlot slot in Slots)
            {
                if (slot.IsClicked())
                {
                    Item slotItem = slot.item;

                    slot.item = heldItem;
                    heldItem = slotItem;
                }
            }
        }

        // Draw methods
        public void DrawInventory(SpriteBatch batch)
        {
            // Draw item slots
            for (int i = 0; i <= 9; i++)
            {
                Slots[i].Draw(batch);
                Slots[i].shown = true;

                for (int j = 0; j <= 3; j++)
                {
                    if (inventoryShown == true)
                    {
                        Slots[i + (j * 10) + 10].Draw(batch);
                        Slots[i + (j * 10) + 10].shown = true;
                    }
                    else
                        Slots[i + (j * 10) + 10].shown = false;
                }
            }

            // Check if a slot is clicked
            foreach (ItemSlot slot in Slots)
            {
                if (!slot.shown)
                    continue;

                int slotIndex = Slots.IndexOf(slot);

                if (slot.item.sprite == null)
                    continue;

                int slotPosX = slotIndex % 10 * 72 + 16;
                int slotPosY = (int)Math.Floor(slotIndex / 10.0f) * 72 + 16;

                batch.Draw(slot.item.sprite, new Vector2(slotPosX, slotPosY), new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
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
                    while (slot.item.amount - 1 >= 0 && amount > 0)
                        amount -= 1;

            if (amount == 0)
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
