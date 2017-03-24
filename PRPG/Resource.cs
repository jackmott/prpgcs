using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PRPG
{
    
    public abstract class Resource : Entity
    {        
        public Texture2D tex;
        public int count;
        public int width;
        public int height;


        public Resource(Vector2 pos, int count, ContentManager content)
        {
            this.count = count;
            this.pos = pos;                        
        }

        public virtual void Draw(Vector2 screenPos)
        {
            PRPGame.batch.Draw(tex, screenPos, Color.White);
        }

        public abstract void Extract(Player player);
        
    }

    public class IronMine : Resource
    {
        public IronMine(Vector2 pos, int count, ContentManager content) : base(pos, count,content)
        {
            width = 128;
            height = 128;
            tex = content.Load<Texture2D>("Resources/iron_mine");

        }

        public override void Extract(Player player)
        {
            count--;
            player.items.Add(new Item("Iron Ore", 1));
        }

    }

    public class CoalMine : Resource
    {
        public CoalMine(Vector2 pos, int count, ContentManager content) : base(pos, count,content)
        {
            width = 128;
            height = 128;
            tex = content.Load<Texture2D>("Resources/mine");
        }

        public override void Extract(Player player)
        {
            count--;
            player.items.Add(new Item("Coal", 1));
        }

    }

    public class Tree : Resource
    {        
        public Tree(Vector2 pos, int count, ContentManager content) : base(pos, count,content)
        {
            width = 32;
            height = 65;
            
            tex = content.Load<Texture2D>("Resources/tree01");
        }

        public override void Extract(Player player)
        {
            count--;
            player.items.Add(new Item("Wood", 1));
        }

    }


}
