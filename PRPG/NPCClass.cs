using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace PRPG
{
              
    public class CraftingRecipe
    {
        public ItemQty[] inputs;
        public ItemQty[] outputs;
        public int time;

        public CraftingRecipe(ItemQty[] inputs, ItemQty[] outputs, int time)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.time = time;
        }
    }
    public class NPCClass
    {
        public string name;
        public Item[] desiredItems;
        public string[] lastNamePrefixes;
        public string[] lastNameSuffixes;
        public CraftingRecipe[] recipes;


        public NPCClass(JObject c)
        {

            name = (string)c["Name"];
            desiredItems = c["Desires"].Select(x => Item.itemPool[(string)x]).ToArray();            
            lastNamePrefixes = c["LastNamePrefix"].Select(x=>(string)x).ToArray();
            lastNameSuffixes= c["LastNameSuffix"].Select(x => (string)x).ToArray();

            var recipesJson = (JArray)c["CraftingRecipes"];
            recipes = new CraftingRecipe[recipesJson.Count];

            for (int i =0; i < recipesJson.Count;i++) {
                var recipe = (JObject)recipesJson[i];
                var inputs = GetItemQtys((JArray)recipe["Inputs"]);
                var ouputs = GetItemQtys((JArray)recipe["Outputs"]);
                int time = (int)recipe["Time"];
                recipes[i] = new CraftingRecipe(inputs, ouputs, time);
            }
        }

        public static ItemQty[] GetItemQtys(JArray objs)
        {
            var itemQtys = new ItemQty[objs.Count];
            for (int i =0; i < objs.Count;i++) {
                var o = (JObject)objs[i];
                itemQtys[i] = new ItemQty((int)o["Qty"],new Item((string)o["Item"]));
            }
            return itemQtys;
        }
    }
}
