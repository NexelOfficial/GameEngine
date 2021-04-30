using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public static class UI
    {
        public static Vector2 GetUiPosition(Player player, Vector2 uiPosition)
        {
            Vector2 cameraPos = new Vector2(player.Position.X + (player.Size.X / 2), player.Position.Y + (player.Size.Y / 2));
            Vector2 uiStart = new Vector2(cameraPos.X - (GameDemo.screenWidth / 2) * (1 / GameDemo.scaleFactor), cameraPos.Y - (GameDemo.screenHeight / 2) * (1 / GameDemo.scaleFactor));

            return uiStart;// + uiPosition;
        }
    }
}
