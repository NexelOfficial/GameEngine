using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.ItemTools
{
    public class Recipe
    {
        public Item result;
        public List<Item> materials;

        public Recipe(Item result, List<Item> materials)
        {
            this.result = result;
            this.materials = materials;
        }
    }
}
