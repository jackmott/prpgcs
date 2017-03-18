using System.Linq;
using Newtonsoft.Json.Linq;

namespace PRPG
{
    public class NPCClass
    {
        public string name;
        public Item[] desiredItems;
        public string[] lastNamePrefixes;
        public string[] lastNameSuffixes;

      
        public NPCClass(JObject c)
        {

            name = (string)c["Name"];
            desiredItems = c["Desires"].Select(x => Item.itemPool[(string)x]).ToArray();            
            lastNamePrefixes = c["LastNamePrefix"].Select(x=>(string)x).ToArray();
            lastNameSuffixes= c["LastNameSuffix"].Select(x => (string)x).ToArray();
        }
    }
}
