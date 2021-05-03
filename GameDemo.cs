using GameEngine.ItemTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class GameDemo : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch, hudBatch;

        public static Player player;
        Camera camera;
        Random rand = new Random();

        public static int screenWidth;
        public static int screenHeight;
        public static int mapWidth = 4000;
        public static int mapHeight = 1500;
        public static int mapSurface = (int) (mapHeight * 0.75);
        public static float scaleFactor = 2.5f;
        public static int seed;

        public static Dictionary<Vector2, Dictionary<Vector2, Tile>> chunks = new Dictionary<Vector2, Dictionary<Vector2, Tile>>();

        public GameDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // Create camera, player, inventory and sprites
            camera = new Camera();
            player = new Player();

            Sprites.InitSprites(Content);
            Items.InitItems();
            Tiles.InitTiles();
            player.inventory.AddItem(Items.GetItem("Iron_Pickaxe"));
            player.inventory.UnlockedBlueprints.Add(new Blueprint(Tiles.GetTile("Furnace"), new List<Item> { Items.GetItem("Stone") }));
            
            // Load the generator and seed
            seed = rand.Next(0, 99999);

            Generator gen = new Generator();

            gen.seed = seed;
            gen.GenerateTerrain(mapWidth, mapHeight);

            rand = new Random(seed);

            // General variables
            screenHeight = graphics.PreferredBackBufferHeight;
            screenWidth = graphics.PreferredBackBufferWidth;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create spritebatches
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudBatch = new SpriteBatch(GraphicsDevice);

            // Add the player
            player.SetPos(mapWidth/2, GetHighestY(mapWidth/2) - 12);
            player.sprite = Content.Load<Texture2D>("Textures/Misc/Player_Size");
        }

        protected override void Update(GameTime gameTime)
        {
            // Update camera, player and selected inventory slot
            camera.Update(gameTime, player);
            player.Update(gameTime);
            player.inventory.SelectSlot();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Game Objects
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);

            foreach (Vector2 chunk in player.playerChunks)
                foreach (Vector2 pos in chunks[chunk].Keys)
                    GetTile(pos).Draw(spriteBatch, pos, Color.White);

            DrawBlueprint();

            player.Draw(spriteBatch);
            spriteBatch.End();

            // Hud
            hudBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            player.inventory.DrawInventory(hudBatch);

            //Texture2D texture = Sprites.GetSprite(0, "Stone");

            //foreach (Vector2 chunk in GetChunkGroup(new Vector2(50, 50), 50))
            //    foreach (Vector2 pos in chunks[chunk].Keys)
            //    {
            //        Vector2 drawPos = new Vector2((int)pos.X, (int)pos.Y);
            //        Rectangle spriteSize = new Rectangle(GetTileState(pos), 0, 1, 1);
            //        string type = GetTile(pos).type;

            //        hudBatch.Draw(texture, drawPos, spriteSize, Color.White);
            //    }

            hudBatch.End();
            base.Draw(gameTime);
        }


        private void DrawBlueprint()
        {
            if (player.inventory.placingBlueprint != null)
            {
                Tile placingBlueprint = player.inventory.placingBlueprint;
                Vector2 worldSpace = player.GetMousePosition();

                int blockX = (int)(Math.Floor(worldSpace.X / 8) + (placingBlueprint.size.X / 4));
                int blockY = (int)(Math.Floor(worldSpace.Y / 8) + (placingBlueprint.size.Y));
                bool canPlace = CanPlace(new Vector2(blockX, blockY), placingBlueprint);

                spriteBatch.Draw(placingBlueprint.sprite, new Vector2(blockX * 8, (blockY - placingBlueprint.size.Y / 2) * 8), canPlace ? Color.Blue : Color.Red);

                if (Controls.IsPressed("LeftButton") && canPlace)
                {
                    AddTile(blockX, blockY, Tiles.GetTile(placingBlueprint.type));
                    player.inventory.placingBlueprint = null;
                }
            }
        }

        #region ChunkMethods
        public static List<Vector2> GetChunkGroup(Vector2 chunk, int regionX, int regionY)
        {
            List<Vector2> chunkGroup = new List<Vector2>();
            int chunkX = (int)chunk.X;
            int chunkY = (int)chunk.Y;

            for (int i = chunkX - regionX; i <= chunkX + regionX; i++)
                for (int j = chunkY - regionY; j <= chunkY + regionY; j++)
                {
                    Vector2 chunkFinal = new Vector2(i, j);

                    if (chunks.ContainsKey(chunkFinal))
                        chunkGroup.Add(chunkFinal);
                }

            return chunkGroup;
        }
        
        public static List<Vector2> GetChunkGroup(Vector2 chunk, int region)
        {
            return GetChunkGroup(chunk, region, region);
        }
        #endregion

        #region TileMethods
        public static bool CanPlace(Vector2 pos, Tile tile)
        {
            for (int i = 0; i < tile.size.X; i++)
            {
                for (int j = 0; j < tile.size.Y; j++)
                {
                    int x = (int)pos.X + i;
                    int y = (int)pos.Y - j;

                    if (GetTile(x, y).type != null)
                        return false;

                    if (x > Math.Floor(player.Position.X / 8) - player.Size.X / 16 - 1 && 
                        x < Math.Ceiling(player.Position.X / 8) + player.Size.X / 16 + 1 && 
                        y > Math.Floor(player.Position.Y / 8) - player.Size.Y / 16 - 1 && 
                        y < Math.Ceiling(player.Position.Y / 8) + player.Size.Y / 16 + 1)
                        return false;
                }

                if (GetTile((int)pos.X + i, (int)pos.Y + 1).type == null)
                    return false;
            }
            return true;
        }

        public static void AddTile(Vector2 pos, Tile tile)
        {
            int chunkX = (int)Math.Floor(pos.X / 8);
            int chunkY = (int)Math.Floor(pos.Y / 8);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            if (!chunks.ContainsKey(chunk))
                chunks.Add(chunk, new Dictionary<Vector2, Tile>());

            chunks[chunk][pos] = tile;
        }

        public static void AddTile(int x, int y, Tile tile)
        {
            AddTile(new Vector2(x, y), tile);
        }

        public static void RemoveTile(Vector2 pos)
        {
            int chunkX = (int)Math.Floor(pos.X / 8);
            int chunkY = (int)Math.Floor(pos.Y / 8);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            if (!chunks.ContainsKey(chunk))
                return;

            chunks[chunk].Remove(pos);
        }

        public static void RemoveTile(int x, int y)
        {
            RemoveTile(new Vector2(x, y));
        }

        public static Tile GetTile(Vector2 pos)
        {
            int chunkX = (int)Math.Floor(pos.X / 8);
            int chunkY = (int)Math.Floor(pos.Y / 8);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            if (!chunks.ContainsKey(chunk))
                return new Tile();

            Dictionary<Vector2, Tile> chunkBlocks = chunks[chunk];

            if (chunkBlocks.ContainsKey(pos))
                return chunkBlocks[pos];
            else
                return new Tile();
        }
        public static Tile GetTile(int x, int y)
        {
            return GetTile(new Vector2(x, y));
        }

        public static int GetHighestY(int x)
        {
            // Find highest chunk
            int chunkX = (int) Math.Floor(x / 8.0f);
            Vector2 highestChunk = new Vector2(mapWidth, mapHeight);

            foreach (Vector2 chunk in chunks.Keys)
                if (chunk.X == chunkX && chunk.Y < highestChunk.Y)
                    highestChunk = chunk;

            // Find highest block
            int y = 9999999;
            if (chunks.ContainsKey(highestChunk))
                foreach (Vector2 pos in chunks[highestChunk].Keys)
                {
                    if (pos.X != x)
                        continue;

                    if ((int)pos.Y < y)
                        y = (int)pos.Y;
                }

            highestChunk.Y += 1;

            if (chunks.ContainsKey(highestChunk))
                foreach (Vector2 pos in chunks[highestChunk].Keys)
                {
                    if (pos.X != x)
                        continue;

                    if ((int)pos.Y < y)
                        y = (int)pos.Y;
                }

            return y;
        }
        #endregion
    }
}
