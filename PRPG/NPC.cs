using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
    
using static PRPG.ProgrammerArt;

namespace PRPG {

    public class NPCClass
    {
        public string name;
        public List<Item> desiredItems;        
        public List<string> lastNamePrefixes;
        public List<string> lastNameSuffixes;

        public NPCClass(string name, List<Item> desiredItems, List<string> lastNamePrefixes,List<string>lastNameSuffixes)
        {
            this.name = name;
            this.desiredItems = desiredItems;
            this.lastNamePrefixes = lastNamePrefixes;
            this.lastNameSuffixes = lastNameSuffixes;
        }
    }

    public class Desire {
        public Item item;
        public int level;
        public int sufficient;

        public Desire(Item item, int level, int sufficient) {
            this.item = item;
            this.level = level;
            this.sufficient = sufficient;
        }

        public override bool Equals(object obj) {
            var desire = (Desire)obj;
            return item.Equals(desire.item);
        }

        public override int GetHashCode() {
            return item.GetHashCode();
        }
    }

    public enum ENPCState { ROAM, HELLO};
    public enum ECommand { ENTER_HELLO_DIST, LEAVE_HELLO_DIST }

    public struct NPCStateTransition {
        ENPCState currentState;
        ECommand command;

        public NPCStateTransition(ENPCState currentState, ECommand command) {
            this.currentState = currentState;
            this.command = command;
        }
    }

    public class Entity {
        public string firstName;
        public string lastName;

        public string fullName { get { return firstName + " " + lastName; } }

        public Inventory items;        
    }

    public class NPC : Entity {

        private static Dictionary<NPCStateTransition, ENPCState> stateMachine = 
            new Dictionary<NPCStateTransition, ENPCState> {
            { new NPCStateTransition(ENPCState.ROAM,ECommand.ENTER_HELLO_DIST),ENPCState.HELLO },
            { new NPCStateTransition(ENPCState.HELLO,ECommand.LEAVE_HELLO_DIST),ENPCState.ROAM },            
        };

        public static string[] namePool;
        public static NPCClass[] npcPool;
        
        public ENPCState state;
        public const int NPCSize = 32;
        public const float helloDist = 1.0f;        
        public Vector2 pos;
        public Texture2D tex;
        public HashSet<Desire> desires;
        public Color currentColor;

        
        
        public NPC(Vector2 pos) {            
            state = ENPCState.ROAM;            
            this.pos = pos;
            firstName = RandUtil.Index(namePool);
            NPCClass npcClass = RandUtil.Index(npcPool);



            items = new Inventory();
            
            int numItems = RandUtil.Int(0, 5);
            var potentialItems = npcClass.desiredItems;
            for (int i = 0; i < numItems; i++) {
                var item = RandUtil.Index(npcClass.desiredItems);
                items.Add(item);
            }

            desires = new HashSet<Desire>();
            var numDesires = RandUtil.Int(0, 4);
            for (int i = 0; i < numDesires; i++) {
                var item = RandUtil.Index(npcClass.desiredItems);
                var level = RandUtil.Int(1, 10);
                var sufficient = RandUtil.Int(1, 10);
                Desire d = new Desire(item, level,sufficient);
                desires.Add(d);
            }

            var lastNamePrefx = RandUtil.Index(npcClass.lastNamePrefixes);
            var lastNameSuffix = RandUtil.Index(npcClass.lastNameSuffixes);

            lastName = lastNamePrefx + lastNameSuffix;
                 
            
            tex = GetSolidTex(NPCSize, NPCSize, Color.White);
            SetColor();
        }

        public static void Initialize() {
            var names = (JArray)JObject.Parse(File.ReadAllText("Data/names.json"))["Names"];

            namePool = new string[names.Count];
            for (int i = 0; i < namePool.Length;i++) {
                namePool[i] = (string)names[i];
            }

            var classes = (JArray)JObject.Parse(File.ReadAllText("Data/classes.json"))["Classes"];
            npcPool = new NPCClass[classes.Count];
            for (int i = 0; i < classes.Count;i++) {
                var c = (JObject)classes[i];
                string name = (string)c["Name"];
                var jsonDesires = (JArray)c["Desires"];
                var desires = new List<Item>();
                foreach (string desire in jsonDesires) {
                    var item = Item.itemPool[desire];
                    desires.Add(item);
                }

                var prefixes = new List<string>();
                foreach (string s in (JArray)c["LastNamePrefix"]) {
                    prefixes.Add(s);
                }

                var suffixes = new List<string>();
                foreach (string s in (JArray)c["LastNameSuffix"]) {
                    suffixes.Add(s);
                }

                var npcClass = new NPCClass(name, desires, prefixes, suffixes);
                npcPool[i] = npcClass;
            }

        }

        public bool IsTradeAcceptable(Inventory proposedInventory) {
            int before = Happiness();
            int after = Happiness(proposedInventory);
            if (after > before)
                return true;
            else if (after == before) {
                return proposedInventory.TotalCount() > items.TotalCount();
            }
            else {
                return false;
            }
        }

        public double Utility(Desire desire, int count) {
            if (count == 0)
                return 0;
            double t = ((double)count / (double)desire.sufficient) * 4 - 2;            
            double S = 1.0 / (1.0 + Math.Pow(MathHelper.E, -t));
            return S * desire.level;
        }

        public int Happiness(Inventory inventory = null) {
            if (inventory == null)
                inventory = items;

            double totalPossibleUtility = desires.Sum(d => d.level);

            //Bhuddism
            if (totalPossibleUtility== 0.0)
                return 100;

            double utility = desires.Sum(x => Utility(x,inventory.CountItem(x.item)));
                        
            double pct = utility / totalPossibleUtility;
            return (int)Math.Round(pct * 100.0);
        }

        public void SetColor() {
            int happy = Happiness();
            currentColor = Color.Lerp(Color.Blue, Color.Orange, (float)happy / 100.0f);
        }

        public Color GetColor() {
            return currentColor;
        }

        public void AdvanceState(ECommand command) {
            ENPCState newState;
            var trans = new NPCStateTransition(state, command);
            if (stateMachine.TryGetValue(trans, out newState)) {
                state = newState;
            }                
        }

        public void Update(GameTime gameTime, Player player) {

            if (Vector2.Distance(pos, player.pos) <= helloDist) {
                AdvanceState(ECommand.ENTER_HELLO_DIST);
            }
            else {
                AdvanceState(ECommand.LEAVE_HELLO_DIST);
            }
               
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {

            batch.Draw(tex, pos * scale - offset, Color.White);
            if (state == ENPCState.HELLO) {
                batch.DrawString(PRPGame.mainFont, "Hello!", pos * scale - offset, Color.White);
            }            
        }


    }
}
