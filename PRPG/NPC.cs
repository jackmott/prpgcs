using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Newtonsoft.Json.Linq;

using static PRPG.GraphUtils;

namespace PRPG
{

    

    public class NPCPersonality
    {

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

  

    public class Entity {
        public string firstName;
        public string lastName;

        public string fullName { get { return firstName + " " + lastName; } }

        public Inventory items;        
    }

    public class NPC : Entity {

      
        public static string[] namePool;
        public static NPCClass[] npcPool;
        public static Personality[] personalityPool;
        
        public ENPCState state;
        public const int NPCSize = 32;
        public const float helloDist = 2.0f;        
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
            
            //Give them some things relevant to their class
            int numItems = RandUtil.Int(0, 5);            
            for (int i = 0; i < numItems; i++) {                
                items.Add(RandUtil.Index(npcClass.desiredItems));
            }
            
            //Give them some stuff not relevant to their class
            numItems = RandUtil.Int(0, 3);
            var potentialItems = Item.itemPool.Values.ToArray();
            for (int i = 0; i < numItems; i++) {
                items.Add(RandUtil.Index(potentialItems));
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
                npcPool[i] = new NPCClass((JObject)classes[i]);
            }

            var personalities = (JArray)JObject.Parse(File.ReadAllText("Data/personalities.json"))["Personalities"];
            personalityPool = new Personality[personalities.Count];
            for (int i = 0; i < personalities.Count;i++) {
                personalityPool[i] = new Personality((JObject)personalities[i]);
            }

        }

        public bool IsTradeAcceptable(Inventory proposedInventory) {
            int before = Happiness();
            int after = Happiness(proposedInventory);
            if (after > before)
                return true;            
            else 
                return false;
            
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

            double totalPossibleUtility = desires.Sum(d => d.level) 
                + inventory.TotalCount();

            //Bhuddism
            if (totalPossibleUtility== 0.0)
                return 100;

            double utility = desires.Sum(x => Utility(x, inventory.CountItem(x.item))) 
                + inventory.TotalCount();
                        
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
            
            switch (state) {
                case ENPCState.HELLO:
                    if (command == ECommand.LEAVE_HELLO_DIST) state = ENPCState.ROAM;
                    break;
                case ENPCState.ROAM:
                    if (command == ECommand.ENTER_HELLO_DIST) state = ENPCState.HELLO;
                    break;
            }
                        
        }

        public void Update(GameTime gameTime, Player player) {

            if (Vector2.DistanceSquared(pos, player.pos) <= helloDist) {
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
