using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static System.Math;


namespace PRPG
{
    public class Player :Entity {

        public const int PlayerSize = 32;
        public Vector2 pos;
        public Vector2 oldPos;
        public TimeSpan lastAnimationTime;
        
        public Player(Vector2 pos) {
            firstName = "Player One";
            this.pos = pos;
            items = new Inventory();        
            for (int i = 0; i < 5;i++) {
                items.Add(RandUtil.Index(Item.itemPool.Values.ToArray()));
            }            
            sprites = new CharSprites(0, true);
            facing = CharSprites.DOWN;
            lastAnimationTime = TimeSpan.FromMilliseconds(0);
               
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {
            batch.Draw(sprites.spriteSheet,pos * scale- offset,sprites.walking[facing,animIndex], Color.White);
        }


        public void Update(GameTime gameTime) {
            if (pos == oldPos) return;
            lastAnimationTime += gameTime.ElapsedGameTime;
            if (lastAnimationTime.TotalMilliseconds> 100) {
                animIndex = (animIndex + 1) % sprites.walking.GetLength(1);
                lastAnimationTime = TimeSpan.FromMilliseconds(0);
            }            
            Vector2 dir = pos - oldPos;            
            if (Abs(dir.X) > Abs(dir.Y)) {
                if (dir.X > 0) facing = CharSprites.RIGHT;
                else facing = CharSprites.LEFT;            
            } else {
                if (dir.Y > 0) facing = CharSprites.DOWN;
                else facing = CharSprites.UP;

            }
            oldPos = pos;
            
        }



    }
}
