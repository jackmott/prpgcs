using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using static PRPG.ProgrammerArt;

namespace PRPG {

    public class Desire {
        public Item item;
        public int level;

        public Desire(Item item, int level) {
            this.item = item;
            this.level = level;
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
        public string name;
        public Inventory items;        
    }

    public class NPC : Entity {

        private static Dictionary<NPCStateTransition, ENPCState> stateMachine = 
            new Dictionary<NPCStateTransition, ENPCState> {
            { new NPCStateTransition(ENPCState.ROAM,ECommand.ENTER_HELLO_DIST),ENPCState.HELLO },
            { new NPCStateTransition(ENPCState.HELLO,ECommand.LEAVE_HELLO_DIST),ENPCState.ROAM },            
        };
        
        public ENPCState state;
        public const int NPCSize = 32;
        public const float helloDist = 1.0f;        
        public Vector2 pos;
        public Texture2D tex;
        public HashSet<Desire> desires;
        
        public NPC(Vector2 pos) {            
            state = ENPCState.ROAM;
            this.pos = pos;
            name = "Fred";
            items = new Inventory();
            var r = new Random((int)pos.X*100+(int)pos.Y*10);
            int numItems = r.Next(0, 10);
            var itemPoolArray = Item.itemPool.Values.ToArray();
            for (int i = 0; i < numItems; i++) {
                var item = itemPoolArray[r.Next(0, Item.itemPool.Count)];
                items.Add(item);
            }

            desires = new HashSet<Desire>();
            var numDesires = r.Next(0, 4);
            for (int i = 0; i < numDesires; i++) {
                var item = itemPoolArray[r.Next(0, Item.itemPool.Count)];
                var level = r.Next(1, 100);
                Desire d = new Desire(item, level);
                desires.Add(d);
            }
            
            tex = GetSolidTex(NPCSize, NPCSize, Color.White);
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

        public int Happiness(Inventory inventory = null) {
            if (inventory == null)
                inventory = items;

            int totalDesire = desires.Sum(x => x.level);
            if (totalDesire == 0)
                return 100;

            int desiresSatisfied = desires.Sum(x => Math.Min(inventory.CountItem(x.item), x.level));
                        
            float pct = (float)desiresSatisfied / (float)totalDesire;
            return (int)Math.Round(pct * 100.0);
        }

        public Color GetColor() {
            int happy = Happiness();
            return Color.Lerp(Color.Blue, Color.Orange, (float)happy / 100.0f);
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
