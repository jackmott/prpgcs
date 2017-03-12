using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static PRPG.ProgrammerArt;

namespace PRPG {
    public enum ENPCState { ROAM, HELLO, DIALOG };
    public enum ECommand { ENTER_HELLO_DIST, LEAVE_HELLO_DIST, TALK_BUTTON }

    public struct NPCStateTransition {
        ENPCState currentState;
        ECommand command;

        public NPCStateTransition(ENPCState currentState, ECommand command) {
            this.currentState = currentState;
            this.command = command;
        }
    }

    public class NPC {

        private static Dictionary<NPCStateTransition, ENPCState> stateMachine = 
            new Dictionary<NPCStateTransition, ENPCState> {
            { new NPCStateTransition(ENPCState.ROAM,ECommand.ENTER_HELLO_DIST),ENPCState.HELLO },
            { new NPCStateTransition(ENPCState.HELLO,ECommand.LEAVE_HELLO_DIST),ENPCState.ROAM },
            { new NPCStateTransition(ENPCState.HELLO,ECommand.TALK_BUTTON),ENPCState.DIALOG},
            { new NPCStateTransition(ENPCState.DIALOG,ECommand.TALK_BUTTON),ENPCState.HELLO}
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
            tex = GetSolidTex(NPCSize, NPCSize, Color.Purple);
        }

        public void AdvanceState(ECommand command) {
            ENPCState newState;
            var trans = new NPCStateTransition(state, command);
            if (stateMachine.TryGetValue(trans, out newState)) {
                state = newState;
            }                
        }

        public ENPCState Update(GameTime gameTime, Player player, bool talkButton) {

            if (Vector2.Distance(pos, player.pos) <= helloDist) {
                AdvanceState(ECommand.ENTER_HELLO_DIST);
            }
            else {
                AdvanceState(ECommand.LEAVE_HELLO_DIST);
            }

            if (talkButton) {
                AdvanceState(ECommand.TALK_BUTTON);
            }

            return state;
               
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {
            batch.Draw(tex, pos * scale - offset, Color.White);
            if (state == ENPCState.HELLO) {
                batch.DrawString(PRPGame.mainFont, "Hello!", pos * scale - offset, Color.White);
            }
            else if (state == ENPCState.DIALOG) {
                batch.DrawString(PRPGame.mainFont, "DIALOGUE!", pos * scale - offset, Color.White);
            }
        }


    }
}
