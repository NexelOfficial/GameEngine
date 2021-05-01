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
        Player player;
        Camera camera;
        Random rand = new Random();
        Noise noise = new Noise();

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
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
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

            // Load the generator and seed
            seed = rand.Next(0, 99999);

            Generator gen = new Generator();

            gen.seed = seed;
            gen.GenerateTerrain(mapWidth, mapHeight);

            AddTile(2000, GetHighestY(2000) - 2, Tiles.GetTile("Furnace"));

            rand = new Random(seed);
            noise = new Noise();

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
                    Draw(pos);

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
        public void Draw(Vector2 pos)
        {
            Tile tile = GetTile(pos);

            for (int i = 0; i < tile.size.X; i++)
                for (int j = 0; j < tile.size.Y; j++)
                {
                    Vector2 drawPos = new Vector2(i * 8 + pos.X * 8, (pos.Y * 8 + j * 8) - (tile.size.Y - 1) * 8);
                    Rectangle spriteSize = new Rectangle();

                    if (tile.size.X == 1 && tile.size.Y == 1)
                        spriteSize = new Rectangle(GetTileState(pos), 0, 8, 8);
                    else
                        spriteSize = new Rectangle(i * 8, j * 8, 8, 8);

                    spriteBatch.Draw(Sprites.GetSprite(0, tile.type), drawPos, spriteSize, Color.White);
                }
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

        public int GetTileState(Vector2 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;

            bool up = false, down = false, left = false, right = false;

            string type = GetTile(x, y).type;

            if (GetTile(x, y - 1).type != null)
                up = true;
            if (GetTile(x, y + 1).type != null)
                down = true;
            if (GetTile(x + 1, y).type != null)
                right = true;
            if (GetTile(x - 1, y).type != null)
                left = true;


            if (left && right && up && down)
                return 1 * 8;
            if (up && down && right)
                return 0;
            if (up && down && left)
                return 2 * 8;
            if (right && down && left)
                return 3 * 8;
            if (right && up && left)
                return 4 * 8;
            if (left && down)
                return 8 * 8;
            if (right && down)
                return 7 * 8;
            if (left && up)
                return 6 * 8;
            if (right && up)
                return 5 * 8;
            if (left && right)
                return 13 * 8;
            if (up && down)
                return 14 * 8;
            if (left)
                return 9 * 8;
            if (right)
                return 10 * 8;
            if (up)
                return 11 * 8;
            if (down)
                return 12 * 8;
            if (!down && !up && !right && !left)
                return 15 * 8;

            return 15 * 8;
        }
        #endregion
    }
}
