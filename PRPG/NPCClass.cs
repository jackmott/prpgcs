using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PRPG
{
              
    public class CraftingRecipe
    {
        public Item[] inputs;
        public Item[] outputs;
        public int time;

        public CraftingRecipe(Item[] inputs, Item[] outputs, int time)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.time = time;
        }
    }

    public class NPCClass
    {
        public string name;
        public string[] desires;
        public string[] lastNamePrefixes;
        public string[] lastNameSuffixes;
        public CraftingRecipe[] craftingRecipes;


        public NPCClass()
        {            
        }

        public static Item[] GetItemQtys(JArray objs)
        {
            var items = new Item[objs.Count];
            for (int i =0; i < objs.Count;i++) {
                var o = (JObject)objs[i];
                items[i] = new Item((string)o["Item"], (int)o["Qty"]);
            }
            return items;
        }
    }
}
