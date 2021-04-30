using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.IO;

namespace GameEngine.ItemTools
{
    public static class Sprites
    {
        public static Dictionary<string, Texture2D> blockSprites = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> itemSprites = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> miscSprites = new Dictionary<string, Texture2D>();

        public static void InitSprites(ContentManager contentManager)
        {
            DirectoryInfo blockDir = new DirectoryInfo(contentManager.RootDirectory + "/Textures/Blocks");
            DirectoryInfo itemDir = new DirectoryInfo(contentManager.RootDirectory + "/Textures/Items");
            DirectoryInfo miscDir = new DirectoryInfo(contentManager.RootDirectory + "/Textures/Misc");

            foreach (FileInfo file in blockDir.GetFiles("*.*"))
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                blockSprites[key] = contentManager.Load<Texture2D>("Textures/Blocks/" + key);
            }

            foreach (FileInfo file in itemDir.GetFiles("*.*"))
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                itemSprites[key] = contentManager.Load<Texture2D>("Textures/Items/" + key);
            }

            foreach (FileInfo file in miscDir.GetFiles("*.*"))
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                miscSprites[key] = contentManager.Load<Texture2D>("Textures/Misc/" + key);
            }
        }

        public static Texture2D GetSprite(int type, string name)
        {
            if (type == 0)
                if (blockSprites.ContainsKey(name))
                    return blockSprites[name];
            if (type == 1)
                if (itemSprites.ContainsKey(name))
                    return itemSprites[name];
            if (type == 2)
                if (miscSprites.ContainsKey(name))
                    return miscSprites[name];

            return null;
        }
    }
}
