using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using static PRPG.GraphUtils;
namespace PRPG
{
    public class Player :Entity {

        public const int PlayerSize = 32;
        public Vector2 pos;
        public Texture2D tex;

        public Player(Vector2 pos) {
            firstName = "Player One";
            this.pos = pos;
            items = new Inventory();        
            for (int i = 0; i < 5;i++) {
                items.Add(RandUtil.Index(Item.itemPool.Values.ToArray()));
            }
            tex = GetSolidTex(PlayerSize, PlayerSize, Color.Red);
        }

        public void Draw(SpriteBatch batch, float scale, Vector2 offset) {
            batch.Draw(tex,pos * scale- offset, Color.White);
        }


        public void Update(GameTime gameTime) {

        }



    }
}
