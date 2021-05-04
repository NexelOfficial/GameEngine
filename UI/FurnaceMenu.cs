using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.ItemTools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.UI
{
    public class FurnaceMenu
    {
        public ItemSlot result = new ItemSlot(Vector2.Zero, new Item());
        public ItemSlot oreOne = new ItemSlot(Vector2.Zero, new Item());
        public ItemSlot oreTwo = new ItemSlot(Vector2.Zero, new Item());
        public ItemSlot oreThree = new ItemSlot(Vector2.Zero, new Item());

        public Vector2 position;

        public FurnaceMenu(Inventory inventory)
        {
            float posX = GameDemo.screenWidth / 2 - 288;
            float posY = GameDemo.screenHeight / 2 - 192;
            this.position = new Vector2(posX, posY);

            oreOne.position = new Vector2(posX + 128, posY + 64);
            oreTwo.position = new Vector2(posX + 256, posY + 64);
            oreThree.position = new Vector2(posX + 384, posY + 64);
            result.position = new Vector2(posX + 256, posY + 256);

            inventory.Slots.Add(oreOne);
            inventory.Slots.Add(oreTwo);
            inventory.Slots.Add(oreThree);
            inventory.Slots.Add(result);
        }

        public void Destroy(Vector2 pos)
        {
            Inventory inventory = GameDemo.player.inventory;
            inventory.Slots.Remove(oreOne);
            inventory.Slots.Remove(oreTwo);
            inventory.Slots.Remove(oreThree);
            inventory.Slots.Remove(result);
            GameDemo.furnaces.Remove(pos);
        }

        public void Draw(SpriteBatch batch)
        {
            ItemSlot[] slots = { oreOne, oreTwo, oreThree, result };
            batch.Draw(Sprites.GetSprite(2, "Furnace"), new Vector2(position.X, position.Y), Color.White);

            // Draw slots
            foreach (ItemSlot slot in slots)
                slot.Draw(batch);

            // Check if a bar can be crafted
            if (oreOne.item.type == oreTwo.item.type && oreTwo.item.type == oreThree.item.type)
            {
                result.item.type = oreOne.item.type;
                result.item.sprite = oreOne.item.sprite;
                result.item.amount += 1;
                oreOne.item.amount -= 1;
                oreTwo.item.amount -= 1;
                oreThree.item.amount -= 1;
            }
        }
    }
}
