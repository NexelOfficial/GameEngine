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
                    GameDemo.player.inventory.blueprintsShown = false;
                    GameDemo.player.inventory.placingBlueprint = blueprint.result;
                }
            }
        }
    }
}
