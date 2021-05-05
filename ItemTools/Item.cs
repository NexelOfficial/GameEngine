using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.ItemTools
{
    public class Item
    {
        public string type;
        public string block;
        public Texture2D sprite;

        public int amount;
        public int miningPower;
        public int waitTime;

        public Item(string type, int amount = 1, int waitTime = 1, int miningPower = 0, string block = null)
        {
            this.sprite = Sprites.GetSprite(1, type);
            this.type = type;
            this.amount = amount;
            this.miningPower = miningPower;
            this.waitTime = waitTime;
            this.block = block;
        }

        public Item()
        {
            this.amount = 0;
        }

        public void Draw(SpriteBatch batch, Vector2 pos, float scale = 3f)
        {
            Vector2 textPos = new Vector2(pos.X + 32 / 3 * scale, pos.Y + 32 / 3 * scale);

            batch.DrawString(GameDemo.font, amount.ToString(), textPos, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            batch.Draw(sprite, pos, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.9f);
        }
    }
}
