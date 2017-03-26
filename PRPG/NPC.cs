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
            return item.name.Equals(desire.item.name);
        }

        public override int GetHashCode() {
            return item.name.GetHashCode();
        }
    }

              
    public abstract class Entity {
        public Vector2 pos;       
    }

    public class NPC : Entity {

        public TimeSpan lastAnimationTime;
        public Vector2 oldPos;
        public string firstName;
        public string lastName;
        public int animIndex;
        public CharSprites sprites;
        public int facing;
        public Gender gender;
        public string fullName { get { return firstName + " " + lastName; } }

        public Inventory inventory;

        public static string[] namePool;
        public static NPCClass[] classPool;
        public static Personality[] personalityPool;


        public Personality personality;        
        public NPCState state;
        public NPCClass npcClass;
        public const int NPCSize = 32;        
        public HashSet<Desire> desires;
        public Color currentColor;

        public bool onScreen = false;
        public bool hello = false;

        public Vector2 destination = Vector2.Zero;

                
        
        public NPC(Vector2 pos, ContentManager content) {
            state = new RestingState();            
            this.pos = pos;
            firstName = RandUtil.Index(namePool);
            npcClass = RandUtil.Index(classPool);
            lastAnimationTime = TimeSpan.FromMilliseconds(0);
            inventory = new Inventory();

            facing = CharSprites.DOWN;

            if (RandUtil.Bool()) {
                gender = Gender.Male;
            } else {
                gender = Gender.Female;
            }
            
            //Give them some things relevant to their class
            int numItems = RandUtil.Int(0, 5);            
            for (int i = 0; i < numItems; i++) {                
                inventory.Add(new Item(RandUtil.Index(npcClass.desires),1));
            }
            
           

            desires = new HashSet<Desire>();
            var numDesires = RandUtil.Int(0, 4);
            for (int i = 0; i < numDesires; i++) {
                var item = RandUtil.Index(npcClass.desires);
                var level = RandUtil.Int(1, 10);
                var sufficient = RandUtil.Int(1, 10);
                Desire d = new Desire(new Item(item,0), level,sufficient);
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
            
            var classes = (JArray)JObject.Parse(File.ReadAllText("Data/classes.json"))["NPCClasses"];
            classPool = classes.Select(x => x.ToObject<NPCClass>()).ToArray();
            
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
                inventory = this.inventory;

            double totalPossibleUtility = desires.Sum(d => d.level)
                + inventory.TotalItems;

            //Bhuddism
            if (totalPossibleUtility== 0.0)
                return 100;

            double utility = desires.Sum(x => Utility(x, inventory.ItemQty(x.item.name)))
                + inventory.TotalItems;
                        
            double pct = utility / totalPossibleUtility;
            return (int)Math.Round(pct * 100.0);
        }

          
        private void CheckIfDoneCrafting(CraftingState craftingState)
        {
            var elapsedTime = DateTime.Now - craftingState.startTime;
            if (elapsedTime.TotalSeconds >= craftingState.recipe.time) {
                foreach (var input in craftingState.recipe.inputs) {
                    inventory.Remove(input);
                }

                foreach (var output in craftingState.recipe.outputs) {
                    inventory.Add(new Item(output.name,output.qty));
                }

                state = new RestingState();
            }
        }

        private void CheckIfCanCraft()
        {
            foreach (var recipe in npcClass.craftingRecipes) {
                bool canCraftThis = true;
                foreach (var item in recipe.inputs) {
                    if (!inventory.Contains(item)) {
                        canCraftThis = false;
                        break;
                    } else {
                        if (inventory.ItemQty(item.name) < item.qty) {
                            canCraftThis = false;
                            break;
                        }
                    }
                }
                if (canCraftThis) state = new CraftingState(recipe);
            }

        }

        public void Update(GameTime gameTime, Player player, ContentManager content) {
            var distSquared = Vector2.DistanceSquared(pos, player.pos);            
            if (distSquared <= PRPGame.maxDist * PRPGame.maxDist) {
                onScreen = true;
                if (distSquared < PRPGame.actionDist) {
                    hello = true;
                } else {
                    hello = false;
                }
                
            } else {
                onScreen = false;
                hello = false;
            }

            if (onScreen && sprites == null) {
                sprites = new CharSprites(gender, content);
            }

            switch (state) {
                case CraftingState crafting:
                    CheckIfDoneCrafting(crafting);
                    break;
                case RestingState resting:
                    CheckIfCanCraft();
                    break;
                default:
                    break;
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

        public void Draw(Vector2 screenPos, float scale = 1.0f, bool onTop = false) {

            if (sprites != null) {                
                Rectangle srcRectangle = sprites.walkingAnimation[facing, animIndex];                
                float depth = 1.0f - (screenPos.Y + srcRectangle.Height) / PRPGame.windowHeight;
                if (onTop) depth = 0.0f;
                PRPGame.batch.Draw(sprites.baseSheet, screenPos,  srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000006f);
                PRPGame.batch.Draw(sprites.eyeSheet, screenPos,   srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000005f);
                PRPGame.batch.Draw(sprites.hairSheet, screenPos,  srcRectangle, sprites.hairColor, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000004f);
                PRPGame.batch.Draw(sprites.shirtSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000003f);
                PRPGame.batch.Draw(sprites.pantSheet, screenPos,  srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000002f);
                PRPGame.batch.Draw(sprites.shoeSheet, screenPos,  srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000001f);
                if (hello && !onTop) {
                    PRPGame.DrawString(PRPGame.mainFont, "Hello!", screenPos, Color.White, 0.02f);                    
                }
            }
        }


    }
}
