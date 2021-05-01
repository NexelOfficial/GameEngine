using GameEngine.ItemTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Player
    {
        public Vector2 Position = new Vector2(0, 0);
        public Vector2 Velocity = new Vector2(0, 0);
        public Vector2 Size = new Vector2(15, 30);

        public Inventory inventory;
        public string type;
        public Texture2D sprite;
        public Color color = Color.Gray;
        
        public float speed = 3f;
        public float jumpHeight = 6f;
        public int health = 100;

        public TimeSpan cooldown;
        public TimeSpan lastAction;

        public List<Vector2> playerChunks;

        public Rectangle collisionBox
        {
            get
            { 
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }   

        public Player()
        {
            inventory = new Inventory(this);
        }

        public void SetPos(int x, int y)
        {
            Position = new Vector2(x * 8, y * 8);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(sprite, Position, color);
        }

        public void Update(GameTime gameTime)
        {
            float scalingFactorX = GameDemo.screenWidth / 160 / (GameDemo.scaleFactor / 2);
            float scalingFactorY = GameDemo.screenHeight / 160 / (GameDemo.scaleFactor / 2);
            playerChunks = GameDemo.GetChunkGroup(GetChunk(), (int)scalingFactorX, (int)scalingFactorY);
            Move();
            Break(gameTime);
            Build(gameTime);

            Velocity.Y += 0.3f;

            if (Velocity.Y > 6f)
                Velocity.Y = 6f;

            Collide();

            Position += Velocity;
        }

        #region PlayerActions
        private void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Velocity.X = -speed;
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
                Velocity.X = speed;
            else
                Velocity.X = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsGrounded())
                Velocity.Y = -jumpHeight;
        }

        private void Break(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (lastAction + cooldown < gameTime.TotalGameTime)
                {
                    // Check if the player's holding item has enough mining power
                    int selectedSlot = inventory.selectedIndex;
                    Item selectedItem = inventory.Slots.ElementAtOrDefault(selectedSlot).item;

                    if (selectedItem == null)
                        return;

                    int miningPower = selectedItem.miningPower;

                    if (miningPower <= 0)
                        return;

                    // Check if block is close enough
                    Vector2 worldSpace = GetMousePosition();
                    int blockX = (int)Math.Floor(worldSpace.X / 8) + 1;
                    int blockY = (int)Math.Floor(worldSpace.Y / 8) + 2;
                    Tile minedTile = GameDemo.GetTile(blockX, blockY);
                    float distance = Vector2.Distance(worldSpace, Position);

                    if (distance > 60)
                        return;

                    // Block takes damage
                    minedTile.hitPoints -= miningPower;
                    lastAction = gameTime.TotalGameTime;
                    cooldown = TimeSpan.FromMilliseconds(selectedItem.waitTime*100);

                    if (minedTile.hitPoints <= 0)
                    {
                        // Block is mined
                        if (minedTile.type != null)
                            inventory.AddItem(Items.GetItem(minedTile.type));

                        GameDemo.RemoveTile(blockX, blockY);
                    }
                }
            }
        }

        private void Build(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                // Check if the player is holding an item
                int selectedSlot = inventory.selectedIndex;
                Item selectedItem = inventory.Slots.ElementAtOrDefault(selectedSlot).item;

                if (selectedItem == null)
                    return;

                if (selectedItem.block == null)
                    return;

                // Check if block is close enough
                Vector2 worldSpace = GetMousePosition();
                int blockX = (int)Math.Floor(worldSpace.X / 8) + 1;
                int blockY = (int)Math.Floor(worldSpace.Y / 8) + 2;
                float distance = Vector2.Distance(worldSpace, Position);
                Tile currentTile = GameDemo.GetTile(blockX, blockY);

                if (distance > 60 || currentTile.type != null)
                    return;

                bool connected = false;

                string type = GameDemo.GetTile(blockX, blockY).type;

                if (GameDemo.GetTile(blockX, blockY - 1).type != null)
                    connected = true;
                if (GameDemo.GetTile(blockX, blockY + 1).type != null)
                    connected = true;
                if (GameDemo.GetTile(blockX + 1, blockY).type != null)
                    connected = true;
                if (GameDemo.GetTile(blockX - 1, blockY).type != null)
                    connected = true;

                if (!connected)
                    return;

                // Place block
                lastAction = gameTime.TotalGameTime;
                cooldown = TimeSpan.FromMilliseconds(selectedItem.waitTime * 100);

                if (inventory.HasItem(selectedItem.type, 1))
                {
                    inventory.RemoveItem(selectedItem.type, 1);
                    GameDemo.AddTile(new Vector2(blockX, blockY), Tiles.GetTile(selectedItem.block));
                }
            }
        }

        private void Collide()
        {
            foreach (Vector2 chunk in playerChunks)
                foreach (Vector2 pos in GameDemo.chunks[chunk].Keys)
                {
                    Tile tile = GameDemo.chunks[chunk][pos];
                    if (IsTouchingBottom(GetTileCollider(pos.X * 8, pos.Y * 8 - Velocity.Y, tile), collisionBox) || IsTouchingTop(GetTileCollider(pos.X * 8, pos.Y * 8 - Velocity.Y, tile), collisionBox))
                        Velocity = new Vector2(Velocity.X, 0);
                    if (IsTouchingLeft(GetTileCollider(pos.X * 8 - Velocity.X, pos.Y * 8, tile), collisionBox) || IsTouchingRight(GetTileCollider(pos.X * 8 - Velocity.X, pos.Y * 8, tile), collisionBox))
                        Velocity = new Vector2(0, Velocity.Y);
                }
        }
        #endregion

        #region HelpMethods
        public Vector2 GetChunk()
        {
            int chunkX = (int)Math.Floor(Position.X / 64);
            int chunkY = (int)Math.Floor(Position.Y / 64);
            // Console.WriteLine(chunkX);
            return new Vector2(chunkX, chunkY);
        }

        public Vector2 GetMousePosition()
        {
            float x = Mouse.GetState().X;
            float y = Mouse.GetState().Y;

            float worldX = (x / GameDemo.screenWidth - 0.5f) * 2f;
            float worldY = (y / GameDemo.screenHeight - 0.5f) * 2f;

            int finalX = (int)Math.Floor(worldX * (GameDemo.screenWidth / 2) / GameDemo.scaleFactor);
            int finalY = (int)Math.Floor(worldY * (GameDemo.screenHeight / 2) / GameDemo.scaleFactor);

            return new Vector2(finalX + Position.X, finalY + Position.Y);
        }
        #endregion

        #region Collision
        public static Rectangle GetTileCollider(float x, float y, Tile tile)
        {
            return new Rectangle((int)x, (int)y - ((int)tile.size.Y - 1) * 8, (int)tile.size.X * 8, (int)tile.size.Y * 8);
        }
        public bool IsTouchingLeft(Rectangle objectOne, Rectangle objectTwo)
        {
            return objectOne.Right > objectTwo.Left &&
                objectOne.Left < objectTwo.Left &&
                objectOne.Bottom > objectTwo.Top &&
                objectOne.Top < objectTwo.Bottom;
        }
        public bool IsTouchingRight(Rectangle objectOne, Rectangle objectTwo)
        {
            return objectOne.Left < objectTwo.Right &&
                objectOne.Right > objectTwo.Right &&
                objectOne.Bottom > objectTwo.Top &&
                objectOne.Top < objectTwo.Bottom;
        }
        public bool IsTouchingTop(Rectangle objectOne, Rectangle objectTwo)
        {
            return objectOne.Bottom > objectTwo.Top &&
                objectOne.Right > objectTwo.Left &&
                objectOne.Top < objectTwo.Top &&
                objectOne.Left < objectTwo.Right;
        }
        public bool IsTouchingBottom(Rectangle objectOne, Rectangle objectTwo)
        {
            return objectOne.Top < objectTwo.Bottom &&
                objectOne.Right > objectTwo.Left &&
                objectOne.Bottom > objectTwo.Bottom &&
                objectOne.Left < objectTwo.Right;
        }
        public bool IsGrounded()
        {
            foreach (Vector2 chunk in playerChunks)
                foreach (Vector2 pos in GameDemo.chunks[chunk].Keys)
                {
                    Rectangle tileCollider = new Rectangle((int)(pos.X * 8), (int)(pos.Y * 8) - 2, 8, 8);

                    if (IsTouchingBottom(tileCollider, collisionBox))
                        return true;
                }

            return false;
        }
        #endregion
    }
}
