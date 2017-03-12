using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static PRPG.ProgrammerArt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPG {
    public static class Dialogue {


        public static Texture2D dialogBackground;
        public static Texture2D selectionBackground;
        private static List<string> options;
        private static int selection = 0;

        public static void Initialize() {
            dialogBackground = GetSolidTex(1, 1, Color.Black);
            selectionBackground = GetSolidTex(1, 1, Color.Blue);
            options = new List<string>();
            options.Add("Trade");
            options.Add("Hello");
            options.Add("WhatUp");
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

            PRPGame.batch.Draw(dialogBackground, new Rectangle(left, top, right - left, bottom - top), Color.White);
            for (int i = 0; i < options.Count; i++) {
                if (i == selection) {
                    PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10, top + 10 + i * 20,100,20), Color.White);
                }
                PRPGame.batch.DrawString(PRPGame.mainFont, options[i], new Vector2(left + 10, top + 10+i*20), Color.White);
            }
            
        }
    }
}
