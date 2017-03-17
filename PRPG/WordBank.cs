using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


namespace PRPG
{
    public class WordBank
    {

        public Dictionary<string, Noun> Nouns {
            get;
            private set;
        }

        public Dictionary<string, Verb> Verbs {
            get;
            private set;
        }

        public Dictionary<string, Adjective> Adjectives {
            get;
            private set;
        }

        public Dictionary<string, Adverb> Adverbs {
            get;
            private set;
        }

        public WordBank()
        {

            Noun[] nouns = JsonConvert.DeserializeObject<Noun[]>(File.ReadAllText("Data/nouns.json"));
            Verb[] verbs = JsonConvert.DeserializeObject<Verb[]>(File.ReadAllText("Data/verbs.json"));
            Adjective[] adjectives = JsonConvert.DeserializeObject<Adjective[]>(File.ReadAllText("Data/adjectives.json"));
            Adverb[] adverbs = JsonConvert.DeserializeObject<Adverb[]>(File.ReadAllText("Data/adverbs.json"));

            Nouns = new Dictionary<string, Noun>();
            Verbs = new Dictionary<string, Verb>();
            Adjectives = new Dictionary<string, Adjective>();
            Adverbs = new Dictionary<string, Adverb>();

            foreach (var noun in nouns) {
                Nouns.Add(noun.Word, noun);
            }

            foreach (var verb in verbs) {
                Verbs.Add(verb.Word, verb);
            }

            foreach (var adjective in adjectives) {
                Adjectives.Add(adjective.Word, adjective);
            }

            foreach (var adverb in adverbs) {
                Adverbs.Add(adverb.Word, adverb);
            }


        }

        public Noun QueryNoun(string wordCase)
        {
            string word = wordCase.ToLower();
            Nouns.TryGetValue(word, out var value);
            return value;                        
        }

        public string QueryNounQty(string wordCase, int qty)
        {
            string word = wordCase.ToLower();
            if (Nouns.ContainsKey(word)) {
                Noun noun = Nouns[word];
                if (qty > 1) {
                    return qty + " " + noun.Plural;
                }
                else if (qty == 1) {
                    return noun.IndefiniteArticle + " " + noun.Word;
                }
                else if (qty == 0) {
                    return "no " + noun.Plural;
                }
            }
            else {
                var wordArray = word.Split(' ');
                var lastWord = wordArray[wordArray.Length - 1];
                if (Nouns.ContainsKey(lastWord)) {
                    string prefix = "";
                    for (int i = 0; i < wordArray.Length - 1; i++) {
                        prefix += wordArray[i];
                    }

                    Noun noun = Nouns[lastWord];
                    if (qty > 1) {
                        return qty + " " + prefix + " " + noun.Plural;
                    }
                    else if (qty == 1) {
                        return noun.IndefiniteArticle + " " + prefix + " " + noun.Word;
                    }
                    else if (qty == 0) {
                        return "no " + prefix + " " + noun.Plural;
                    }
                }


            }
            return null;
        }

        public Adjective QueryAdjective(string word)
        {
            return Adjectives[word];
        }

        public Adverb QueryAdverb(string word)
        {
            return Adverbs[word];
        }

        public Verb QueryVerb(string word)
        {
            return Verbs[word];
        }

        public string QueryVerbPresent(string word)
        {
            if (Verbs.ContainsKey(word)) {
                var verb = Verbs[word];
                return verb.Progressive;
            }
            return null;
        }

        public string QueryVerbPast(string word)
        {
            if (Verbs.ContainsKey(word)) {
                var verb = Verbs[word];
                return verb.Past;
            }
            return null;
        }

    }


    public class Noun
    {
        public string Word {
            get;
            private set;
        }

        public string Plural {
            get;
            private set;
        }

        public string IndefiniteArticle {
            get;
            private set;
        }

        public Noun(string word, string plural, string indefiniteArticle)
        {
            Word = word;
            Plural = plural;
            IndefiniteArticle = indefiniteArticle;
        }

        public override string ToString()
        {
            return "Noun: " + Word + "\nPlural Form: " + Plural + "\nIdefinite Article: " + IndefiniteArticle;
        }

    }


    public class Verb
    {
        public string Word {
            get;
            private set;
        }

        public string Past {
            get;
            private set;
        }

        public string Progressive {
            get;
            private set;
        }

        public string PastPerfect {
            get;
            private set;
        }

        public string IndefiniteArticle {
            get;
            private set;
        }

        public Verb(string word, string past, string progressive, string pastPerfect, string indefiniteArticle)
        {
            Word = word;
            PastPerfect = pastPerfect;
            Past = past;
            Progressive = progressive;
            IndefiniteArticle = indefiniteArticle;
        }

        public override string ToString()
        {
            return "Verb: " + Word + "\nPast: " + Past + "\nProgressive: " + Progressive + "\nPast Perfect: " + PastPerfect + "\nIdefinite Article: " + IndefiniteArticle;
        }

    }


    public class Adjective
    {
        public string Word {
            get;
            private set;
        }


        public string IndefiniteArticle {
            get;
            private set;
        }

        public Adjective(string word, string indefiniteArticle)
        {
            Word = word;
            IndefiniteArticle = indefiniteArticle;
        }

        public override string ToString()
        {
            return "Adjective: " + Word + "\nIdefinite Article: " + IndefiniteArticle;
        }

    }

    public class Adverb
    {
        public string Word {
            get;
            private set;
        }


        public string IndefiniteArticle {
            get;
            private set;
        }

        public Adverb(string word, string indefiniteArticle)
        {
            Word = word;
            IndefiniteArticle = indefiniteArticle;
        }

    }



}