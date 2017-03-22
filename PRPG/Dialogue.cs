using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static PRPG.GraphUtils;
using System.Linq;


namespace PRPG
{
    public static class Dialogue {


        public static Texture2D dialogBackground;
        public static Texture2D selectionBackground;
        private static List<string> options;
        public static string currentResponse = string.Empty;
        private static int selection = 0;

        public static void Initialize() {
            dialogBackground = GetSolidTex(1, 1, Color.Black);
            selectionBackground = GetSolidTex(1, 1, Color.Blue);
            options = new List<string>();
            options.Add("Let's Trade");
            options.Add("Tell me about yourself.");            
        }

        public static void Setup() {
            currentResponse = string.Empty;
        }

        public static GameCommand Selection() {
            if (selection == 0) {
                return GameCommand.TRADE;
            }
            else {
                if (PRPGame.closestNPC.desires.Count > 0) {
                    var randomDesire = RandUtil.Index(PRPGame.closestNPC.desires.ToArray());
                    currentResponse = PRPGame.closestNPC.personality.GetLikeResponse(randomDesire.item);
                }
                else {
                    currentResponse = "There isn't much to say really...";
                }
                return GameCommand.NONE;
            }
        }

        public static void IncrementSelection() {
            selection++;
            if (selection == options.Count)
                selection--;
        }

        public static void DecrementSelection() {
            selection--;
            if (selection < 0) selection = 0;
        }

        public static void Draw() {
            int left = (int)(PRPGame.windowWidth * 0.1f);
            int right = (int)(PRPGame.windowWidth * 0.9f);
            int top = (int)(PRPGame.windowHeight * 0.1f);
            int bottom = (int)(PRPGame.windowHeight * 0.9f);

            int w = right - left;

            PRPGame.batch.Draw(dialogBackground, new Rectangle(left, top, right - left, bottom - top), Color.White);
            int strLen = (int)PRPGame.mainFont.MeasureString(PRPGame.closestNPC.fullName).X;
            PRPGame.batch.DrawString(PRPGame.mainFont, PRPGame.closestNPC.fullName, new Vector2(w / 2.0f + left - strLen / 2.0f, top),Color.LightBlue);
            top += 30;
            for (int i = 0; i < options.Count; i++) {
                if (i == selection) {
                    PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10, top + 10 + i * 20,400,20), Color.White);
                }
                PRPGame.batch.DrawString(PRPGame.mainFont, options[i], new Vector2(left + 10, top + 10+i*20), Color.White);
            }

            if (currentResponse != string.Empty) {
                strLen = (int)PRPGame.mainFont.MeasureString(currentResponse).X;
                PRPGame.batch.DrawString(PRPGame.mainFont, currentResponse, new Vector2(w / 2.0f + left - strLen/2.0f, bottom - 100), Color.LightBlue);
            }

            
        }
    }
}
