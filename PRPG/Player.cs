using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static System.Math;


namespace PRPG
{
    public class Player : Entity
    {

        public TimeSpan lastAnimationTime;
        public Vector2 oldPos;
        public string firstName;
        public string lastName;
        public int animIndex;
        public CharSprites sprites;
        public int facing;
        public Gender gender;
        public string fullName { get { return firstName + " " + lastName; } }
        public bool chopping = false;
        public Inventory inventory;
        public const int PlayerSize = 32;

        public Resource resourceBeingActedOn;
        public World world;



        public Player(Vector2 pos, World world, ContentManager content)
        {
            firstName = "Player One";
            this.world = world;
            this.pos = pos;
            inventory = new Inventory();
            inventory.Add(new Item("Iron Ore", 100));
            gender = Gender.Male;

            sprites = new CharSprites(gender, content);

            facing = CharSprites.DOWN;
            lastAnimationTime = TimeSpan.FromMilliseconds(0);

        }

        public void Draw(Vector2 screenPos, float scale = 1.0f, bool onTop = false)
        {
            if (!chopping)
            {
                Rectangle srcRectangle = sprites.walkingAnimation[facing, animIndex];
                float depth = 1.0f - (screenPos.Y + srcRectangle.Height) / PRPGame.windowHeight;
                if (onTop) depth = 0.0f;
                PRPGame.batch.Draw(sprites.baseSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000006f);
                PRPGame.batch.Draw(sprites.eyeSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000005f);
                PRPGame.batch.Draw(sprites.hairSheet, screenPos, srcRectangle, sprites.hairColor, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000004f);
                PRPGame.batch.Draw(sprites.shirtSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000003f);
                PRPGame.batch.Draw(sprites.pantSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000002f);
                PRPGame.batch.Draw(sprites.shoeSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000001f);
            }
            else
            {
                Rectangle srcRectangle = sprites.swingAnimation[facing, animIndex];
                float depth = 1.0f - (screenPos.Y + srcRectangle.Height) / PRPGame.windowHeight;
                if (onTop) depth = 0.0f;
                PRPGame.batch.Draw(sprites.baseSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000006f);
                PRPGame.batch.Draw(sprites.eyeSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000005f);
                PRPGame.batch.Draw(sprites.hairSheet, screenPos, srcRectangle, sprites.hairColor, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000004f);
                PRPGame.batch.Draw(sprites.shirtSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000003f);
                PRPGame.batch.Draw(sprites.pantSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000002f);
                PRPGame.batch.Draw(sprites.shoeSheet, screenPos, srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth + .000001f);

                srcRectangle = sprites.oversizeWeaponAnimation[facing, animIndex];
                PRPGame.batch.Draw(CharSprites.saberSheet, screenPos - new Vector2(64,64), srcRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth);
            }
        }

        public void Chop(Resource resource)
        {
            resourceBeingActedOn = resource;
            chopping = true;
            animIndex = 0;
        }
        public void FinishChop()
        {
            resourceBeingActedOn.Extract(this);
            if (resourceBeingActedOn.count == 0)
            {
                world.resources.Remove(resourceBeingActedOn);
                world.deadResources.Add(resourceBeingActedOn);
            }
            chopping = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!chopping) AnimateWalking(gameTime);
            else AnimateSwinging(gameTime);

        }

        private void AnimateSwinging(GameTime gameTime)
        {            
            lastAnimationTime += gameTime.ElapsedGameTime;
            if (lastAnimationTime.TotalMilliseconds > 100)
            {
                animIndex++;
                if (animIndex == CharSprites.SWING_WIDTH)
                {
                    animIndex = 0;
                    if (chopping)
                    {
                        FinishChop();
                    }
                }
                lastAnimationTime = TimeSpan.FromMilliseconds(0);
            }
            
        }

        private void AnimateWalking(GameTime gameTime)
        {
            if (pos == oldPos) return;
            lastAnimationTime += gameTime.ElapsedGameTime;
            if (lastAnimationTime.TotalMilliseconds > 100)
            {
                animIndex = (animIndex + 1) % CharSprites.WALKING_WIDTH;
                lastAnimationTime = TimeSpan.FromMilliseconds(0);
            }


            Vector2 dir = pos - oldPos;
            if (Abs(dir.X) > Abs(dir.Y))
            {
                if (dir.X > 0) facing = CharSprites.RIGHT;
                else facing = CharSprites.LEFT;
            }
            else
            {
                if (dir.Y > 0) facing = CharSprites.DOWN;
                else facing = CharSprites.UP;

            }
            oldPos = pos;
        }



    }
}
