using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.ItemTools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.UI
{
    public class CraftingMenu
    {
        public void Draw(SpriteBatch batch)
        {
            List<Recipe> allRecipes = GameDemo.player.inventory.unlockedRecipes;

            foreach (Recipe recipe in allRecipes)
            {
                // Draw recipe
                int listPos = allRecipes.IndexOf(recipe);

                int column = listPos % 6;
                int row = (int)Math.Floor(listPos / 6.0f);

                float posX = GameDemo.screenWidth / 2 - 222 + (column * 74);
                float posY = GameDemo.screenHeight / 2 - 222 + (row * 74);

                recipe.position = new Vector2(posX, posY);
                recipe.Draw(batch);

                // Check if clicked
                if (recipe.IsClicked())
                {
                    Inventory inventory = GameDemo.player.inventory;

                    bool canCraft = true;
                    foreach (Item material in recipe.materials)
                        if (!inventory.HasItem(material.type, material.amount))
                            canCraft = false;

                    if (canCraft)
                    {
                        foreach (Item material in recipe.materials)
                            inventory.RemoveItem(material.type, material.amount);

                        if (inventory.heldItem.type == null || inventory.heldItem.type == recipe.result.type)
                        {
                            inventory.heldItem.type = recipe.result.type;
                            inventory.heldItem.sprite = recipe.result.sprite;
                            inventory.heldItem.amount += recipe.result.amount;
                        }
                    }
                }
            }
        }
    }
}
