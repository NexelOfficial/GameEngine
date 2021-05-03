using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.ItemTools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.UI
{
    public class BlueprintsMenu
    {
        public void Draw(SpriteBatch batch)
        {
            List<Blueprint> allBlueprints = GameDemo.player.inventory.UnlockedBlueprints;

            foreach (Blueprint blueprint in allBlueprints)
            {
                // Draw blueprint
                int listPos = allBlueprints.IndexOf(blueprint);

                float posX = GameDemo.screenWidth / 2 - 256;
                float posY = GameDemo.screenHeight / 2 - 32 + (listPos * 74);

                blueprint.position = new Vector2(posX, posY);
                blueprint.Draw(batch);

                // Check if clicked
                if (blueprint.IsClicked())
                {
                    Inventory inventory = GameDemo.player.inventory;

                    bool canCraft = true;
                    foreach (Item material in blueprint.materials)
                        if (!inventory.HasItem(material.type, material.amount))
                            canCraft = false;

                    if (canCraft)
                    {
                        foreach (Item material in blueprint.materials)
                            inventory.RemoveItem(material.type, material.amount);

                        inventory.blueprintsShown = false;
                        inventory.placingBlueprint = blueprint.result;
                    }
                }
            }
        }
    }
}
