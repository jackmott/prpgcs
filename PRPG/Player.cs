﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static System.Math;


namespace PRPG
{
    public class Player :Entity {

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
            batch.Draw(sprites.spriteSheet,pos * scale- offset - new Vector2(32.0f,64.0f),sprites.walking[facing,animIndex], Color.White);
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
