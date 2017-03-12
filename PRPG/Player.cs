using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static PRPG.ProgrammerArt;
namespace PRPG {
    public class Player {

        public const int PlayerSize = 32;
        public Vector2 pos;
        public Texture2D tex;
        public List<string> items;

        public Player(Vector2 pos) {
            this.pos = pos;
            items = new List<string>();
            items.Add("Gold");
            tex = GetSolidTex(PlayerSize, PlayerSize, Color.Red);
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {
            batch.Draw(tex,pos * scale- offset, Color.White);
        }


        public void Update(GameTime gameTime) {

        }



    }
}
