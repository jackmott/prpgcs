using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static System.Math;


namespace PRPG
{
    public class Player : Entity {

        public TimeSpan lastAnimationTime;
        public Vector2 oldPos;
        public string firstName;
        public string lastName;
        public int animIndex;
        public CharSprites sprites;
        public int facing;
        public Gender gender;
        public string fullName { get { return firstName + " " + lastName; } }

        public Inventory items;
        public const int PlayerSize = 32;
       
        
        
        public Player(Vector2 pos, ContentManager content) {
            firstName = "Player One";
            this.pos = pos;
            items = new Inventory();
            items.Add(new Item("Iron Ore", 100));
            gender = Gender.Male;
            
            sprites = new CharSprites(gender,content);
            
            facing = CharSprites.DOWN;
            lastAnimationTime = TimeSpan.FromMilliseconds(0);
               
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {
            var screenPos = pos * scale - offset;
            Rectangle srcRectangle = sprites.walking[facing, animIndex];
            float depth = 1.0f - (screenPos.Y + srcRectangle.Height)/PRPGame.windowHeight;            
            PRPGame.batch.Draw(sprites.baseSheet,screenPos ,sprites.walking[facing, animIndex],Color.White, 0, Vector2.Zero,1, SpriteEffects.None,depth+.06f);
            PRPGame.batch.Draw(sprites.eyeSheet, screenPos,sprites.walking[facing, animIndex], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth+0.05f);
            PRPGame.batch.Draw(sprites.hairSheet, screenPos, sprites.walking[facing, animIndex],sprites.hairColor, 0, Vector2.Zero, 1, SpriteEffects.None, depth+.04f);
            PRPGame.batch.Draw(sprites.shirtSheet, screenPos,sprites.walking[facing, animIndex],Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth+.03f);
            PRPGame.batch.Draw(sprites.pantSheet, screenPos,sprites.walking[facing, animIndex],Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth+.02f);
            PRPGame.batch.Draw(sprites.shoeSheet, screenPos,sprites.walking[facing, animIndex],Color.White,0, Vector2.Zero, 1, SpriteEffects.None, depth+.01f);
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
