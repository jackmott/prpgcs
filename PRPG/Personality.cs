using System.Linq;
using Newtonsoft.Json.Linq;

namespace PRPG
{
    public class Personality
    {
        public string name;
        public string[] like;
        public string[] dontLike;
        public string[] haveEnough;
        public string[] needMore;

        public Personality(JObject p)
        {
            name = (string)p["name"];
            like = p["Like"].Select(x => (string)x).ToArray();
            dontLike = p["DontLike"].Select(x => (string)x).ToArray();
            haveEnough = p["HaveEnough"].Select(x => (string)x).ToArray();
            needMore = p["NeedMore"].Select(x => (string)x).ToArray();
        }
    }
}
