using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class Camera
    {
        public Matrix Transform;
        public Matrix HudTransform;

        public void Update(GameTime gameTime, Player player)
        {
            var position = Matrix.CreateTranslation(-player.Position.X - (player.Size.X / 2), -player.Position.Y - (player.Size.Y / 2), 0);
            var offset = Matrix.CreateTranslation(GameDemo.screenWidth / 2, GameDemo.screenHeight / 2, 0);
            var scale = Matrix.CreateScale(GameDemo.scaleFactor);

            Transform = (position * scale) * offset;
        }
    }
}
