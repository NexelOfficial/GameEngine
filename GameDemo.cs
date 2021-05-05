using GameEngine.ItemTools;
using GameEngine.UI;
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
        public static SpriteFont font;

        Camera camera;
        Random rand = new Random();

        public static int screenWidth;
        public static int screenHeight;
        public static int mapWidth = 4000;
        public static int mapHeight = 1500;
        public static int mapSurface = (int) (mapHeight * 0.75);
        public static float scaleFactor = 2.8f;
        public static int chunkSize = 8;
        public static Vector2 chunkScalingFactor;
        public static int seed;

        public static Dictionary<Vector2, Dictionary<Vector2, Tile>> chunks = new Dictionary<Vector2, Dictionary<Vector2, Tile>>();
        public static Dictionary<Vector2, FurnaceMenu> furnaces = new Dictionary<Vector2, FurnaceMenu>();
        public static Dictionary<Vector2, Light> lights = new Dictionary<Vector2, Light>();

        public GameDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            screenHeight = graphics.PreferredBackBufferHeight;
            screenWidth = graphics.PreferredBackBufferWidth;
            chunkScalingFactor = new Vector2(screenWidth / (20 * chunkSize) / (scaleFactor / 2f), screenWidth / (35 * chunkSize) / (scaleFactor / 2f));
        }

        protected override void Initialize()
        {
            Program.TryForceHighPerformanceGpu();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create camera, player, inventory, sprites and fonts
            camera = new Camera();
            player = new Player();

            Sprites.InitSprites(Content);
            Items.InitItems();
            Tiles.InitTiles();
            font = Content.Load<SpriteFont>("segoe");

            // Starter items
            player.inventory.AddItem(Items.GetItem("IronPickaxe"));
            player.inventory.AddItem(Items.GetItem("IronBar", 50));

            // Blueprints
            player.inventory.unlockedBlueprints.Add(new Blueprint(
                Tiles.GetTile("Furnace"), new List<Item> 
                { 
                    Items.GetItem("Stone", 20) 
                }
            ));

            // Recipes
            for (int i = 0; i < 20; i++)
                player.inventory.unlockedRecipes.Add(new Recipe(
                    Items.GetItem("IronPickaxe"), new List<Item>
                    {
                        Items.GetItem("IronBar", 5)
                    }
                ));

            // Load the generator and seed
            seed = rand.Next(0, 99999);

            Generator gen = new Generator();

            gen.seed = seed;
            gen.GenerateTerrain(mapWidth, mapHeight);
            //lights.Add(new Light(new Vector2(2000, GetHighestY(2000)), 20));

            rand = new Random(seed);

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

            Controls.GetMouseState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Game Objects
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);

            foreach (Vector2 chunk in player.playerChunks)
                foreach (Vector2 pos in chunks[chunk].Keys)
                    GetTile(pos).Draw(spriteBatch, pos);

            DrawBlueprint();

            CalculateLightLevel();
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

        private void CalculateLightLevel()
        {
            Vector2 chunk = player.GetChunk();
            int startX = (int)(chunk.X - chunkScalingFactor.X + 2) * chunkSize;
            int endX = (int)(chunk.X + chunkScalingFactor.X - 2) * chunkSize + chunkSize;

            // Load sunlight
            for (int x = startX; x <= endX; x++)
            {
                new Light(new Vector2(x, GetHighestY(x)), chunkSize, 0.2f).Render();
            }

            // Load static lights
            foreach (Light light in lights.Values)
                light.Render();
        }

        private void DrawBlueprint()
        {
            if (player.inventory.placingBlueprint != null)
            {
                Tile placingBlueprint = player.inventory.placingBlueprint;
                Vector2 worldSpace = player.GetMousePosition();

                int blockX = (int)(Math.Floor(worldSpace.X / chunkSize) + (placingBlueprint.size.X / 4));
                int blockY = (int)(Math.Floor(worldSpace.Y / chunkSize) + (placingBlueprint.size.Y));
                bool canPlace = CanPlace(new Vector2(blockX, blockY), placingBlueprint);

                spriteBatch.Draw(placingBlueprint.sprite, new Vector2(blockX * chunkSize, (blockY - placingBlueprint.size.Y / 2) * chunkSize), canPlace ? Color.Blue : Color.Red);

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

        public static void AddTile(Vector2 pos, Tile tile)
        {
            int chunkX = (int)Math.Floor(pos.X / chunkSize);
            int chunkY = (int)Math.Floor(pos.Y / chunkSize);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            if (!chunks.ContainsKey(chunk))
                chunks.Add(chunk, new Dictionary<Vector2, Tile>());

            if (tile.type == "Furnace")
                furnaces.Add(pos, new FurnaceMenu(player.inventory));

            chunks[chunk][pos] = tile;
        }

        public static void AddTile(int x, int y, Tile tile)
        {
            AddTile(new Vector2(x, y), tile);
        }

        public static bool CanPlace(Vector2 pos, Tile tile, bool floored = true)
        {
            for (int i = 0; i < tile.size.X; i++)
            {
                for (int j = 0; j < tile.size.Y; j++)
                {
                    int x = (int)pos.X + i;
                    int y = (int)pos.Y - j;

                    if (GetTile(x, y).type != null)
                        return false;

                    if (x > Math.Floor(player.Position.X / chunkSize) - player.Size.X / 16 - 1 &&
                        x < Math.Ceiling(player.Position.X / chunkSize) + player.Size.X / 16 + 1 &&
                        y > Math.Floor(player.Position.Y / chunkSize) - player.Size.Y / 16 - 1 &&
                        y < Math.Ceiling(player.Position.Y / chunkSize) + player.Size.Y / 16 + 1)
                        return false;
                }

                if (floored)
                    if (GetTile((int)pos.X + i, (int)pos.Y + 1).type == null)
                        return false;
            }
            return true;
        }

        public static bool IsEmpty(Vector2 pos, int width, int height)
        {
            for (int i = (int)pos.X; i < pos.X + width; i++)
                for (int j = (int)pos.Y; j < pos.Y + height; j++)
                    if (GetTile(i, j).type != null)
                        return false;

            return true;
        }

        public static void RemoveTile(Vector2 pos, bool drop = false)
        {
            int chunkX = (int)Math.Floor(pos.X / chunkSize);
            int chunkY = (int)Math.Floor(pos.Y / chunkSize);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            if (!chunks.ContainsKey(chunk))
                return;

            if (furnaces.ContainsKey(pos))
                furnaces[pos].Destroy(pos);

            if (drop && chunks[chunk].ContainsKey(pos))
            {
                Tile current = chunks[chunk][pos];
                Console.WriteLine(current.type);
                if (current.type != null)
                    player.inventory.AddItem(Items.GetItem(current.type));
            }

            chunks[chunk].Remove(pos);
        }

        public static void RemoveTile(int x, int y, bool drop = false)
        {
            RemoveTile(new Vector2(x, y), drop);
        }

        public static void RemoveTree(Vector2 pos)
        {
            bool flag = true;
            string woodType = GetTile(pos).type;
            int y = 0;

            // Continue if another layer of logs is found
            while (flag && woodType != null)
            {
                // Place tree top if no more logs are found
                if (GetTile((int)pos.X, (int)pos.Y - y).type != woodType)
                {
                    flag = false;
                    for (int x = -1; x <= 1; x++)
                        RemoveTile((int)pos.X - 5 + x, (int)pos.Y - y, true);
                }

                for (int x = -1; x <= 1; x++)
                    RemoveTile((int)pos.X + x, (int)pos.Y - y, true);
                y++;
            }
        }

        public static Tile GetTile(Vector2 pos)
        {
            int chunkX = (int)Math.Floor(pos.X / chunkSize);
            int chunkY = (int)Math.Floor(pos.Y / chunkSize);
            Vector2 chunk = new Vector2(chunkX, chunkY);

            Dictionary<Vector2, Tile> chunkBlocks;
            chunks.TryGetValue(chunk, out chunkBlocks);

            if (chunkBlocks == null)
                return new Tile();

            Tile output;
            chunkBlocks.TryGetValue(pos, out output);
            if (output != null)
                return output;
            else
                return new Tile();
        }
        public static Tile GetTile(int x, int y)
        {
            return GetTile(new Vector2(x, y));
        }

        public static int GetHighestY(int x)
        {
            for (int y = 0; y < mapHeight; y++)
                if (GetTile(x, y).type != null)
                    return y;

            return 0;    
        }
        #endregion
    }
}
