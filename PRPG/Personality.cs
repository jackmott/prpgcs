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
        public string[] idleChat;
        public string[] goodTrade;
        public string[] badTrade;
        WordBank wordBank;

        public Personality(JObject p, WordBank wordBank)
        {
            this.wordBank = wordBank;
            name = (string)p["Name"];
            like = p["Like"].Select(x => (string)x).ToArray();
            dontLike = p["DontLike"].Select(x => (string)x).ToArray();
            haveEnough = p["HaveEnough"].Select(x => (string)x).ToArray();
            needMore = p["NeedMore"].Select(x => (string)x).ToArray();
            idleChat = p["IdleChat"].Select(x => (string)x).ToArray();
            goodTrade = p["GoodTrade"].Select(x => (string)x).ToArray();
            badTrade = p["BadTrade"].Select(x => (string)x).ToArray();
        }

        public string GetLikeResponse(Item item)
        {
            var response = RandUtil.Index(like);
            var itemWord = item.name.ToLower();
            var noun = wordBank.QueryNoun(itemWord);
            var itemWordPlural = noun.Plural;
            var indefArticle = noun.IndefiniteArticle;
            response = response.Replace("ITEMS", itemWordPlural);
            response = response.Replace("ITEM", itemWord);
            response = response.Replace("ARTICLE", indefArticle);
            response.Trim();
            return response;
        }
    }
}
