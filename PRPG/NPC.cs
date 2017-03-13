using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static PRPG.ProgrammerArt;

namespace PRPG {
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
        public List<Item> items;

        public void AddItem(Item item) {
            var existingItem = items.Find(x => x.name.Equals(item.name));
            if (existingItem != null) {
                existingItem.count++;
            }
            else {
                items.Add(item);
            }            
        }

        public void RemoveItem(Item item) {
            var existingItem = items.Find(x => x.name.Equals(item.name));
            if (existingItem != null) {
                if (existingItem.count == 1)
                    items.RemoveAll(x => x.name.Equals(item.name));
                else
                    existingItem.count--;
            }
            else {
                items.Remove(item);
            }            
        }
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
        public string name;
        public Vector2 pos;
        public Texture2D tex;
       
        

        public NPC(Vector2 pos) {            
            state = ENPCState.ROAM;
            this.pos = pos;
            name = "Fred";
            items = new List<Item>();
            items.Add(new Item(1,"Guns"));
            items.Add(new Item(10,"Butter"));
            tex = GetSolidTex(NPCSize, NPCSize, Color.Purple);
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
