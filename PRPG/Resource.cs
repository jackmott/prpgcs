using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace PRPG
{
    
    public abstract class Resource : Entity
    {        
        public Texture2D tex;
        public Texture2D deadTex;
        public int count;
        public int width;
        public int height;

        public abstract void Draw(Vector2 screenPos);
        

        public abstract void Extract(Player player);
        
    }

    public class IronMine : Resource
    {
        public IronMine(Vector2 pos, int count, ContentManager content) 
        {            
            tex = content.Load<Texture2D>("Resources/iron_mine");
            width = tex.Width;
            height = tex.Height;
            this.pos = pos;
            this.count = count;
        }

        public override void Extract(Player player)
        {
            count--;
            player.inventory.Add(new Item("Iron Ore", 1));
        }

        public override void Draw(Vector2 screenPos)
        {
            float depth = 1.0f - (screenPos.Y +40.0f) / PRPGame.windowHeight;
            depth = MathHelper.Clamp(depth, 0.0f, float.MaxValue);
            PRPGame.batch.Draw(tex, screenPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth); 
        }

    }

    public class CoalMine : Resource
    {
        public CoalMine(Vector2 pos, int count, ContentManager content)
        {            
            tex = content.Load<Texture2D>("Resources/mine");
            width = tex.Width;
            height = tex.Height;
            this.pos = pos;
            this.count = count;
        }

        public override void Extract(Player player)
        {
            count--;
            player.inventory.Add(new Item("Coal", 1));
        }

        public override void Draw(Vector2 screenPos)
        {
            float depth = 1.0f - (screenPos.Y +40.0f) / PRPGame.windowHeight;
            depth = MathHelper.Clamp(depth, 0.0f, float.MaxValue);
            PRPGame.batch.Draw(tex, screenPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth);
        }

    }

    public class Tree : Resource
    {        
        public Tree(Vector2 pos, int count, ContentManager content) 
        {                        
            tex = content.Load<Texture2D>("Resources/tree01");
            deadTex = content.Load<Texture2D>("Resources/tree01_cut");
            width = tex.Width;
            height = tex.Height;
            this.pos = pos;// new Vector2(pos.X - (float)width / 2.0f, pos.Y - (float)height);
            this.pos.Y -= ((float)height/(float)World.tileSize);
            this.pos.X -= ((float)width / (float)World.tileSize / 2.0f);            
            this.count = count;
            Debug.Assert(this.count > 0);
        }

        public override void Extract(Player player)
        {            
            if (count > 0)
            {
                count--;
                player.inventory.Add(new Item("Wood", 1));
                if (count == 0)
                {
                    tex = deadTex;                    
                }
            }         
        }

        public override void Draw(Vector2 screenPos)
        {
            float depth = 1.0f - (screenPos.Y + tex.Height) / PRPGame.windowHeight;
            depth = MathHelper.Clamp(depth, 0.0f, float.MaxValue);
            PRPGame.batch.Draw(tex, screenPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, depth);
        }

    }


}
