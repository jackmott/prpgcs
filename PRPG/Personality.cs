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
        WordBank wordBank;

        public Personality(JObject p, WordBank wordBank)
        {
            this.wordBank = wordBank;
            name = (string)p["Name"];
            like = p["Like"].Select(x => (string)x).ToArray();
            dontLike = p["DontLike"].Select(x => (string)x).ToArray();
            haveEnough = p["HaveEnough"].Select(x => (string)x).ToArray();
            needMore = p["NeedMore"].Select(x => (string)x).ToArray();
        }

        public string GetLikeResponse(Item item)
        {
            var response = RandUtil.Index(like);
            var itemWord = item.name.ToLower();
            var noun = wordBank.QueryNoun(itemWord);
            var itemWordPlural = noun.Plural;
            var indefArticle = noun.IndefiniteArticle;
            response = response.Replace("@", itemWordPlural);
            response = response.Replace("#", itemWord);
            response = response.Replace("%", indefArticle);
            response.Trim();
            return response;
        }
    }
}
