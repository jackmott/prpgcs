using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Newtonsoft.Json.Linq;
using static System.Math;
using Microsoft.Xna.Framework.Content;

namespace PRPG
{
   

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
            if (obj == null) return false;
            var desire = (Desire)obj;
            return item.Equals(desire.item);
        }

        public override int GetHashCode() {
            return item.GetHashCode();
        }
    }

    public enum ENPCState { STILL, TRAVELLING};
    public enum ECommand { ENTER_HELLO_DIST, LEAVE_HELLO_DIST }

  

    public class Entity {
        public TimeSpan lastAnimationTime;
        public Vector2 pos;
        public Vector2 oldPos;
        public string firstName;
        public string lastName;
        public int animIndex;
        public CharSprites sprites;
        public int facing;
        public Gender gender;
        public string fullName { get { return firstName + " " + lastName; } }

        public Inventory items;        
    }

    public class NPC : Entity {        

        public static string[] namePool;
        public static NPCClass[] npcPool;
        public static Personality[] personalityPool;

        public Personality personality;        
        public ENPCState state;
        public const int NPCSize = 32;
        public const float helloDist = 2.0f;                
        public HashSet<Desire> desires;
        public Color currentColor;
        public bool hello = false;

        public Vector2 destination = Vector2.Zero;

        
        
        
        public NPC(Vector2 pos, ContentManager content) {            
            state = ENPCState.STILL;            
            this.pos = pos;
            firstName = RandUtil.Index(namePool);
            NPCClass npcClass = RandUtil.Index(npcPool);
            lastAnimationTime = TimeSpan.FromMilliseconds(0);
            items = new Inventory();
            
            if (RandUtil.Bool()) {
                gender = Gender.Male;
            } else {
                gender = Gender.Female;
            }
            
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

            personality = RandUtil.Index(personalityPool);            
        }

        public static void Initialize() {
            var names = (JArray)JObject.Parse(File.ReadAllText("Data/names.json"))["Names"];


            namePool = names.Select(x => (string)x).ToArray();
            
            var classes = (JArray)JObject.Parse(File.ReadAllText("Data/classes.json"))["Classes"];
            npcPool = classes.Select(x => new NPCClass((JObject)x)).ToArray();
            
            var personalities = (JArray)JObject.Parse(File.ReadAllText("Data/personalities.json"))["Personalities"];
            personalityPool = personalities.Select(x => new Personality((JObject)x, PRPGame.wordBank)).ToArray();
            
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

      

        public void AdvanceState(ECommand command) {    
            if (command == ECommand.ENTER_HELLO_DIST) {
                hello = true;
            } 
            if (command == ECommand.LEAVE_HELLO_DIST) {
                hello = false;
            }            
                        
        }

        public void Update(GameTime gameTime, Player player, ContentManager content) {
            var dist = Vector2.DistanceSquared(pos, player.pos);            
            if (dist <= PRPGame.maxDist * PRPGame.maxDist) {

                if (sprites == null) {              
                    sprites = new CharSprites(gender, content);
                }
            }
            if (dist <= helloDist) {
                AdvanceState(ECommand.ENTER_HELLO_DIST);
            }
            else {
                AdvanceState(ECommand.LEAVE_HELLO_DIST);
            }            

            /* move around code 
            if (destination == Vector2.Zero) {
                if (RandUtil.Dice(1000)) {
                    var destVector = new Vector2(RandUtil.Int(-10,10), RandUtil.Int(-10,10));
                    destination = pos + destVector;
                }
            } else {
                Vector2 toDestination = (destination - pos);
                toDestination.Normalize();
                toDestination *= 0.05f;
                pos += toDestination;
                if (Vector2.Distance(pos, destination) < 0.2) destination = Vector2.Zero;
            }

            if (pos == oldPos) return;
            lastAnimationTime += gameTime.ElapsedGameTime;
            if (lastAnimationTime.TotalMilliseconds > 100) {
                animIndex = (animIndex + 1) % sprites.walking.GetLength(1);
                lastAnimationTime = TimeSpan.FromMilliseconds(0);
            }
            Vector2 dir = pos - oldPos;
            if (Abs(dir.X) > Abs(dir.Y)) {
                if (dir.X > 0) facing = CharSprites.RIGHT;
                else facing = CharSprites.LEFT;
            }
            else {
                if (dir.Y > 0) facing = CharSprites.DOWN;
                else facing = CharSprites.UP;

            }
            oldPos = pos;*/

        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {

            if (sprites != null) {
                batch.Draw(sprites.spriteSheet, pos * scale - offset, sprites.walking[facing, animIndex], Color.White);
                if (hello) {
                    batch.DrawString(PRPGame.mainFont, "Hello!", pos * scale - offset, Color.White);
                }
            }
        }


    }
}
